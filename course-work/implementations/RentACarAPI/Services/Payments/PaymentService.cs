using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Application.Abstractions;
using RentACarAPI.Application.Payments.Contracts;

namespace RentACarAPI.Application.Payments;

public sealed class PaymentService : IPaymentService
{
    private readonly IAppDbContext _dbContext;

    public PaymentService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaymentResponse> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken)
        => await CreateAsync(request, userId: null, cancellationToken);

    public async Task<PaymentResponse> CreateForUserAsync(CreatePaymentRequest request, int userId, CancellationToken cancellationToken)
        => await CreateAsync(request, userId, cancellationToken);

    private async Task<PaymentResponse> CreateAsync(CreatePaymentRequest request, int? userId, CancellationToken cancellationToken)
    {
        if (!PaymentMethod.All.Contains(request.Method))
        {
            throw new InvalidOperationException("Invalid payment method.");
        }

        var rental = await _dbContext.Rentals
            .Include(r => r.Reservation)
            .FirstOrDefaultAsync(r => r.Id == request.RentalId, cancellationToken);

        if (rental is null)
        {
            throw new InvalidOperationException("Rental not found.");
        }

        if (userId is not null && rental.Reservation.UserId != userId.Value)
        {
            throw new UnauthorizedAccessException("You are not allowed to pay this rental.");
        }

        if (!string.Equals(rental.Status, RentalStatus.Closed, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Payment can be created only after the rental is closed.");
        }

        var exists = await _dbContext.Payments.AnyAsync(p => p.RentalId == request.RentalId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("Payment already exists for this rental.");
        }

        var payment = new Payment
        {
            RentalId = request.RentalId,
            Amount = rental.TotalPrice,
            CreatedAt = DateTime.UtcNow,
            Method = request.Method,
            Status = PaymentStatus.Pending
        };

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new PaymentResponse(payment.Id, payment.RentalId, payment.Amount, payment.CreatedAt, payment.Method, payment.Status);
    }

    public async Task<PaymentResponse> GetByRentalIdAsync(int rentalId, CancellationToken cancellationToken)
        => await GetByRentalIdAsync(rentalId, userId: null, cancellationToken);

    public async Task<PaymentResponse> GetByRentalIdForUserAsync(int rentalId, int userId, CancellationToken cancellationToken)
        => await GetByRentalIdAsync(rentalId, userId, cancellationToken);

    private async Task<PaymentResponse> GetByRentalIdAsync(int rentalId, int? userId, CancellationToken cancellationToken)
    {
        var payment = await _dbContext.Payments
            .AsNoTracking()
            .Where(p => p.RentalId == rentalId)
            .Select(p => new
            {
                Payment = p,
                UserId = p.Rental.Reservation.UserId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (payment is null)
        {
            throw new InvalidOperationException("Payment not found.");
        }

        if (userId is not null && payment.UserId != userId.Value)
        {
            throw new UnauthorizedAccessException("You are not allowed to view this payment.");
        }

        var p = payment.Payment;
        return new PaymentResponse(p.Id, p.RentalId, p.Amount, p.CreatedAt, p.Method, p.Status);
    }

    public async Task<IReadOnlyCollection<PaymentResponse>> GetMineAsync(int userId, GetMyPaymentsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Payment> q = _dbContext.Payments.AsNoTracking()
            .Where(p => p.Rental.Reservation.UserId == userId);

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (!PaymentStatus.All.Contains(query.Status))
            {
                throw new InvalidOperationException("Invalid payment status.");
            }

            q = q.Where(p => p.Status == query.Status);
        }

        if (query.From is not null)
        {
            q = q.Where(p => p.CreatedAt >= query.From.Value);
        }

        if (query.To is not null)
        {
            q = q.Where(p => p.CreatedAt <= query.To.Value);
        }

        return await q
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentResponse(p.Id, p.RentalId, p.Amount, p.CreatedAt, p.Method, p.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PaymentResponse>> GetAllAsync(GetPaymentsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Payment> q = _dbContext.Payments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (!PaymentStatus.All.Contains(query.Status))
            {
                throw new InvalidOperationException("Invalid payment status.");
            }

            q = q.Where(p => p.Status == query.Status);
        }

        if (!string.IsNullOrWhiteSpace(query.Method))
        {
            if (!PaymentMethod.All.Contains(query.Method))
            {
                throw new InvalidOperationException("Invalid payment method.");
            }

            q = q.Where(p => p.Method == query.Method);
        }

        if (query.RentalId is not null)
        {
            q = q.Where(p => p.RentalId == query.RentalId.Value);
        }

        if (query.UserId is not null)
        {
            q = q.Where(p => p.Rental.Reservation.UserId == query.UserId.Value);
        }

        if (query.From is not null)
        {
            q = q.Where(p => p.CreatedAt >= query.From.Value);
        }

        if (query.To is not null)
        {
            q = q.Where(p => p.CreatedAt <= query.To.Value);
        }

        return await q
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentResponse(p.Id, p.RentalId, p.Amount, p.CreatedAt, p.Method, p.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<PaymentResponse> MarkPaidAsync(int rentalId, CancellationToken cancellationToken)
        => await MarkPaidAsync(rentalId, userId: null, cancellationToken);

    public async Task<PaymentResponse> MarkPaidForUserAsync(int rentalId, int userId, CancellationToken cancellationToken)
        => await MarkPaidAsync(rentalId, userId, cancellationToken);

    private async Task<PaymentResponse> MarkPaidAsync(int rentalId, int? userId, CancellationToken cancellationToken)
    {
        var payment = await _dbContext.Payments
            .Include(p => p.Rental)
            .ThenInclude(r => r.Reservation)
            .FirstOrDefaultAsync(p => p.RentalId == rentalId, cancellationToken);

        if (payment is null)
        {
            throw new InvalidOperationException("Payment not found.");
        }

        if (userId is not null && payment.Rental.Reservation.UserId != userId.Value)
        {
            throw new UnauthorizedAccessException("You are not allowed to pay this rental.");
        }

        if (string.Equals(payment.Status, PaymentStatus.Paid, StringComparison.OrdinalIgnoreCase))
        {
            return new PaymentResponse(payment.Id, payment.RentalId, payment.Amount, payment.CreatedAt, payment.Method, payment.Status);
        }

        payment.Status = PaymentStatus.Paid;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new PaymentResponse(payment.Id, payment.RentalId, payment.Amount, payment.CreatedAt, payment.Method, payment.Status);
    }
}
