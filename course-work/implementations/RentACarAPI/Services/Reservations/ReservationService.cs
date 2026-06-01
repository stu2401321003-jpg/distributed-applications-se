using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Application.Abstractions;
using RentACarAPI.Application.Reservations.Contracts;

namespace RentACarAPI.Application.Reservations;

public sealed class ReservationService : IReservationService
{
    private readonly IAppDbContext _dbContext;

    public ReservationService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReservationResponse> CreateAsync(CreateReservationRequest request, int userId, CancellationToken cancellationToken)
    {
        if (request.EndDate <= request.StartDate)
        {
            throw new InvalidOperationException("EndDate must be after StartDate.");
        }

        var carExists = await _dbContext.Cars.AnyAsync(c => c.Id == request.CarId && c.IsActive, cancellationToken);
        if (!carExists)
        {
            throw new InvalidOperationException("Car not found or inactive.");
        }

        if (request.TariffPlanId is not null)
        {
            var tariffAssigned = await _dbContext.Cars
                .Where(c => c.Id == request.CarId)
                .AnyAsync(c => c.TariffPlans.Any(t => t.Id == request.TariffPlanId.Value), cancellationToken);

            if (!tariffAssigned)
            {
                throw new InvalidOperationException("Tariff plan is not assigned to this car.");
            }
        }

        var pickupExists = await _dbContext.Locations.AnyAsync(l => l.Id == request.PickupLocationId, cancellationToken);
        if (!pickupExists)
        {
            throw new InvalidOperationException("Pickup location not found.");
        }

        var dropoffExists = await _dbContext.Locations.AnyAsync(l => l.Id == request.DropoffLocationId, cancellationToken);
        if (!dropoffExists)
        {
            throw new InvalidOperationException("Dropoff location not found.");
        }

        var overlaps = await _dbContext.Reservations.AnyAsync(r =>
            r.CarId == request.CarId &&
            r.Status != ReservationStatus.Cancelled &&
            r.Status != ReservationStatus.Rejected &&
            request.StartDate < r.EndDate &&
            request.EndDate > r.StartDate,
            cancellationToken);

        if (overlaps)
        {
            throw new InvalidOperationException("Car is not available for the selected period.");
        }

        var reservation = new Reservation
        {
            UserId = userId,
            CarId = request.CarId,
            PickupLocationId = request.PickupLocationId,
            DropoffLocationId = request.DropoffLocationId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TariffPlanId = request.TariffPlanId,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(reservation);
    }

    public async Task<IReadOnlyCollection<ReservationResponse>> GetMineAsync(int userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Reservations
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReservationResponse(
                r.Id,
                r.UserId,
                r.CarId,
                r.PickupLocationId,
                r.DropoffLocationId,
                r.StartDate,
                r.EndDate,
                r.Status,
                r.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ReservationListItemResponse>> GetAllAsync(GetReservationsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Reservation> q = _dbContext.Reservations.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (!ReservationStatus.All.Contains(query.Status))
            {
                throw new InvalidOperationException("Invalid reservation status.");
            }

            q = q.Where(r => r.Status == query.Status);
        }

        if (query.UserId is not null)
        {
            q = q.Where(r => r.UserId == query.UserId.Value);
        }

        if (query.CarId is not null)
        {
            q = q.Where(r => r.CarId == query.CarId.Value);
        }

        if (query.From is not null)
        {
            q = q.Where(r => r.StartDate >= query.From.Value);
        }

        if (query.To is not null)
        {
            q = q.Where(r => r.EndDate <= query.To.Value);
        }

        return await q
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReservationListItemResponse(
                r.Id,
                r.UserId,
                r.CarId,
                r.PickupLocationId,
                r.DropoffLocationId,
                r.StartDate,
                r.EndDate,
                r.Status,
                r.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<ReservationDetailsResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Reservations
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new ReservationDetailsResponse(
                r.Id,
                r.UserId,
                r.CarId,
                r.PickupLocationId,
                r.DropoffLocationId,
                r.StartDate,
                r.EndDate,
                r.Status,
                r.CreatedAt,
                r.Car.PlateNumber,
                r.Car.Model,
                r.PickupLocation.Name,
                r.DropoffLocation.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            throw new InvalidOperationException("Reservation not found.");
        }

        return item;
    }

    public async Task<ReservationResponse> ApproveAsync(int id, CancellationToken cancellationToken)
    {
        var reservation = await _dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (reservation is null)
        {
            throw new InvalidOperationException("Reservation not found.");
        }

        if (!string.Equals(reservation.Status, ReservationStatus.Pending, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending reservations can be approved.");
        }

        reservation.Status = ReservationStatus.Approved;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(reservation);
    }

    public async Task<ReservationResponse> RejectAsync(int id, CancellationToken cancellationToken)
    {
        var reservation = await _dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (reservation is null)
        {
            throw new InvalidOperationException("Reservation not found.");
        }

        if (!string.Equals(reservation.Status, ReservationStatus.Pending, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending reservations can be rejected.");
        }

        reservation.Status = ReservationStatus.Rejected;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(reservation);
    }

    public async Task CancelAsync(int id, int userId, CancellationToken cancellationToken)
    {
        var reservation = await _dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (reservation is null)
        {
            throw new InvalidOperationException("Reservation not found.");
        }

        if (reservation.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not allowed to cancel this reservation.");
        }

        if (!string.Equals(reservation.Status, ReservationStatus.Pending, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(reservation.Status, ReservationStatus.Approved, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only pending or approved reservations can be cancelled.");
        }

        reservation.Status = ReservationStatus.Cancelled;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static ReservationResponse Map(Reservation reservation)
        => new(
            reservation.Id,
            reservation.UserId,
            reservation.CarId,
            reservation.PickupLocationId,
            reservation.DropoffLocationId,
            reservation.StartDate,
            reservation.EndDate,
            reservation.Status,
            reservation.CreatedAt);
}
