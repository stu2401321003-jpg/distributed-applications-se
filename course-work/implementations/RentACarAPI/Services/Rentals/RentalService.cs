using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Application.Abstractions;
using RentACarAPI.Application.Rentals.Contracts;
using RentACarAPI.Application.Rentals.Pricing;

namespace RentACarAPI.Application.Rentals;

public sealed class RentalService : IRentalService
{
    private readonly IAppDbContext _dbContext;

    public RentalService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RentalResponse> CreateFromReservationAsync(CreateRentalRequest request, CancellationToken cancellationToken)
    {
        var reservation = await _dbContext.Reservations
            .Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.Id == request.ReservationId, cancellationToken);

        if (reservation is null)
        {
            throw new InvalidOperationException("Reservation not found.");
        }

        if (!string.Equals(reservation.Status, ReservationStatus.Approved, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only approved reservations can be converted to rentals.");
        }

        var existingRental = await _dbContext.Rentals.AnyAsync(r => r.ReservationId == request.ReservationId, cancellationToken);
        if (existingRental)
        {
            throw new InvalidOperationException("Rental already exists for this reservation.");
        }

        var startActual = request.StartActual ?? DateTime.UtcNow;

        var rental = new Rental
        {
            ReservationId = reservation.Id,
            StartActual = startActual,
            StartMileage = request.StartMileage,
            Status = RentalStatus.Open,
            TotalPrice = 0m
        };

        _dbContext.Rentals.Add(rental);

        reservation.Status = ReservationStatus.InProgress;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RentalResponse(rental.Id, rental.ReservationId, rental.StartActual, rental.EndActual, rental.StartMileage, rental.EndMileage, rental.Status, rental.TotalPrice);
    }

    public async Task<RentalResponse> CloseAsync(int rentalId, CloseRentalRequest request, CancellationToken cancellationToken)
    {
        var rental = await _dbContext.Rentals
            .Include(r => r.Reservation)
            .ThenInclude(res => res.Car)
            .Include(r => r.Reservation)
            .ThenInclude(res => res.TariffPlan)
            .Include(r => r.RentalExtraServices)
            .ThenInclude(res => res.ExtraService)
            .FirstOrDefaultAsync(r => r.Id == rentalId, cancellationToken);

        if (rental is null)
        {
            throw new InvalidOperationException("Rental not found.");
        }

        if (!string.Equals(rental.Status, RentalStatus.Open, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only open rentals can be closed.");
        }

        var endActual = request.EndActual ?? DateTime.UtcNow;
        if (endActual <= rental.StartActual)
        {
            throw new InvalidOperationException("EndActual must be after StartActual.");
        }

        if (request.EndMileage < rental.StartMileage)
        {
            throw new InvalidOperationException("EndMileage must be greater than or equal to StartMileage.");
        }

        rental.EndActual = endActual;
        rental.EndMileage = request.EndMileage;
        rental.Status = RentalStatus.Closed;

        var days = GetChargedDays(rental.StartActual, endActual);

        var baseTotal = TariffPricing.CalculateBaseTotal(days, rental.Reservation.Car.DailyBasePrice, rental.Reservation.TariffPlan);
        var extrasTotal = CalculateExtrasTotal(rental, days);

        rental.TotalPrice = baseTotal + extrasTotal;

        rental.Reservation.Status = ReservationStatus.Completed;

        var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.RentalId == rental.Id, cancellationToken);
        if (payment is null)
        {
            payment = new Payment
            {
                RentalId = rental.Id,
                Amount = rental.TotalPrice,
                CreatedAt = DateTime.UtcNow,
                Method = PaymentMethod.Cash,
                Status = PaymentStatus.Pending
            };

            _dbContext.Payments.Add(payment);
        }
        else
        {
            payment.Amount = rental.TotalPrice;
            if (string.Equals(payment.Status, PaymentStatus.Paid, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Payment is already paid.");
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RentalResponse(rental.Id, rental.ReservationId, rental.StartActual, rental.EndActual, rental.StartMileage, rental.EndMileage, rental.Status, rental.TotalPrice);
    }

    public async Task<RentalResponse> GetByIdAsync(int rentalId, CancellationToken cancellationToken)
    {
        var rental = await _dbContext.Rentals
            .AsNoTracking()
            .Where(r => r.Id == rentalId)
            .Select(r => new RentalResponse(
                r.Id,
                r.ReservationId,
                r.StartActual,
                r.EndActual,
                r.StartMileage,
                r.EndMileage,
                r.Status,
                r.TotalPrice))
            .FirstOrDefaultAsync(cancellationToken);

        if (rental is null)
        {
            throw new InvalidOperationException("Rental not found.");
        }

        return rental;
    }

    public async Task<IReadOnlyCollection<RentalResponse>> GetMineAsync(int userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Rentals
            .AsNoTracking()
            .Where(r => r.Reservation.UserId == userId)
            .OrderByDescending(r => r.Id)
            .Select(r => new RentalResponse(
                r.Id,
                r.ReservationId,
                r.StartActual,
                r.EndActual,
                r.StartMileage,
                r.EndMileage,
                r.Status,
                r.TotalPrice))
            .ToListAsync(cancellationToken);
    }

    public async Task<RentalResponse> GetMineByIdAsync(int rentalId, int userId, CancellationToken cancellationToken)
    {
        var rental = await _dbContext.Rentals
            .AsNoTracking()
            .Where(r => r.Id == rentalId && r.Reservation.UserId == userId)
            .Select(r => new RentalResponse(
                r.Id,
                r.ReservationId,
                r.StartActual,
                r.EndActual,
                r.StartMileage,
                r.EndMileage,
                r.Status,
                r.TotalPrice))
            .FirstOrDefaultAsync(cancellationToken);

        if (rental is null)
        {
            throw new InvalidOperationException("Rental not found.");
        }

        return rental;
    }

    public async Task<IReadOnlyCollection<RentalResponse>> GetAllAsync(GetRentalsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Rental> q = _dbContext.Rentals.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (!RentalStatus.All.Contains(query.Status))
            {
                throw new InvalidOperationException("Invalid rental status.");
            }

            q = q.Where(r => r.Status == query.Status);
        }

        if (query.ReservationId is not null)
        {
            q = q.Where(r => r.ReservationId == query.ReservationId.Value);
        }

        if (query.UserId is not null)
        {
            q = q.Where(r => r.Reservation.UserId == query.UserId.Value);
        }

        if (query.From is not null)
        {
            q = q.Where(r => r.StartActual >= query.From.Value);
        }

        if (query.To is not null)
        {
            q = q.Where(r => r.EndActual != null && r.EndActual <= query.To.Value);
        }

        return await q
            .OrderByDescending(r => r.Id)
            .Select(r => new RentalResponse(
                r.Id,
                r.ReservationId,
                r.StartActual,
                r.EndActual,
                r.StartMileage,
                r.EndMileage,
                r.Status,
                r.TotalPrice))
            .ToListAsync(cancellationToken);
    }

    public async Task<RentalDetailsResponse> GetDetailsAsync(int rentalId, CancellationToken cancellationToken)
    {
        var rental = await LoadRentalWithExtrasAsync(rentalId, cancellationToken);
        return MapDetails(rental);
    }

    public async Task<RentalDetailsResponse> GetMyDetailsAsync(int rentalId, int userId, CancellationToken cancellationToken)
    {
        var rental = await LoadRentalWithExtrasAsync(rentalId, cancellationToken);
        if (rental.Reservation.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to view this rental.");
        }

        return MapDetails(rental);
    }

    public async Task<RentalDetailsResponse> AddExtraAsync(int rentalId, AddRentalExtraServiceRequest request, CancellationToken cancellationToken)
        => await AddExtraAsync(rentalId, userId: null, request, cancellationToken);

    public async Task<RentalDetailsResponse> AddExtraForUserAsync(int rentalId, int userId, AddRentalExtraServiceRequest request, CancellationToken cancellationToken)
        => await AddExtraAsync(rentalId, userId, request, cancellationToken);

    private async Task<RentalDetailsResponse> AddExtraAsync(int rentalId, int? userId, AddRentalExtraServiceRequest request, CancellationToken cancellationToken)
    {
        var rental = await LoadRentalWithExtrasAsync(rentalId, cancellationToken);

        if (userId is not null && rental.Reservation.UserId != userId.Value)
        {
            throw new UnauthorizedAccessException("You are not allowed to modify this rental.");
        }

        if (!string.Equals(rental.Status, RentalStatus.Open, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Extras can be modified only while rental is open.");
        }

        var extraExists = await _dbContext.ExtraServices.AnyAsync(es => es.Id == request.ExtraServiceId, cancellationToken);
        if (!extraExists)
        {
            throw new InvalidOperationException("Extra service not found.");
        }

        var existing = rental.RentalExtraServices.FirstOrDefault(x => x.ExtraServiceId == request.ExtraServiceId);
        if (existing is null)
        {
            rental.RentalExtraServices.Add(new RentalExtraService
            {
                RentalId = rental.Id,
                ExtraServiceId = request.ExtraServiceId,
                Quantity = request.Quantity
            });
        }
        else
        {
            existing.Quantity += request.Quantity;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        rental = await LoadRentalWithExtrasAsync(rentalId, cancellationToken);
        return MapDetails(rental);
    }

    public async Task<RentalDetailsResponse> RemoveExtraAsync(int rentalId, int extraServiceId, CancellationToken cancellationToken)
        => await RemoveExtraAsync(rentalId, userId: null, extraServiceId, cancellationToken);

    public async Task<RentalDetailsResponse> RemoveExtraForUserAsync(int rentalId, int userId, int extraServiceId, CancellationToken cancellationToken)
        => await RemoveExtraAsync(rentalId, userId, extraServiceId, cancellationToken);

    private async Task<RentalDetailsResponse> RemoveExtraAsync(int rentalId, int? userId, int extraServiceId, CancellationToken cancellationToken)
    {
        var rental = await LoadRentalWithExtrasAsync(rentalId, cancellationToken);

        if (userId is not null && rental.Reservation.UserId != userId.Value)
        {
            throw new UnauthorizedAccessException("You are not allowed to modify this rental.");
        }

        if (!string.Equals(rental.Status, RentalStatus.Open, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Extras can be modified only while rental is open.");
        }

        var existing = rental.RentalExtraServices.FirstOrDefault(x => x.ExtraServiceId == extraServiceId);
        if (existing is null)
        {
            return MapDetails(rental);
        }

        rental.RentalExtraServices.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);

        rental = await LoadRentalWithExtrasAsync(rentalId, cancellationToken);
        return MapDetails(rental);
    }

    private async Task<Rental> LoadRentalWithExtrasAsync(int rentalId, CancellationToken cancellationToken)
    {
        var rental = await _dbContext.Rentals
            .Include(r => r.Reservation)
            .ThenInclude(res => res.Car)
            .Include(r => r.RentalExtraServices)
            .ThenInclude(res => res.ExtraService)
            .FirstOrDefaultAsync(r => r.Id == rentalId, cancellationToken);

        if (rental is null)
        {
            throw new InvalidOperationException("Rental not found.");
        }

        return rental;
    }

    private static decimal GetChargedDays(DateTime start, DateTime end)
    {
        var days = (decimal)Math.Ceiling((end - start).TotalDays);
        return days < 1 ? 1 : days;
    }

    private static decimal CalculateExtrasTotal(Rental rental, decimal days)
    {
        return rental.RentalExtraServices.Sum(x => x.Quantity * x.ExtraService.PricePerDay * days);
    }

    private static RentalDetailsResponse MapDetails(Rental rental)
    {
        var end = rental.EndActual ?? DateTime.UtcNow;
        var days = GetChargedDays(rental.StartActual, end);

        var baseTotal = days * rental.Reservation.Car.DailyBasePrice;
        var extrasTotal = CalculateExtrasTotal(rental, days);
        var total = rental.Status.Equals(RentalStatus.Closed, StringComparison.OrdinalIgnoreCase) ? rental.TotalPrice : baseTotal + extrasTotal;

        var extras = rental.RentalExtraServices
            .OrderBy(x => x.ExtraServiceId)
            .Select(x => new RentalExtraServiceResponse(x.ExtraServiceId, x.ExtraService.Name, x.ExtraService.PricePerDay, x.Quantity))
            .ToList();

        return new RentalDetailsResponse(
            rental.Id,
            rental.ReservationId,
            rental.StartActual,
            rental.EndActual,
            rental.StartMileage,
            rental.EndMileage,
            rental.Status,
            baseTotal,
            extrasTotal,
            total,
            extras);
    }
}
