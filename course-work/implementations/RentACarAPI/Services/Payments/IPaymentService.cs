using RentACarAPI.Application.Payments.Contracts;

namespace RentACarAPI.Application.Payments;

public interface IPaymentService
{
    Task<PaymentResponse> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken);
    Task<PaymentResponse> CreateForUserAsync(CreatePaymentRequest request, int userId, CancellationToken cancellationToken);

    Task<PaymentResponse> GetByRentalIdAsync(int rentalId, CancellationToken cancellationToken);
    Task<PaymentResponse> GetByRentalIdForUserAsync(int rentalId, int userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PaymentResponse>> GetMineAsync(int userId, GetMyPaymentsQuery query, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PaymentResponse>> GetAllAsync(GetPaymentsQuery query, CancellationToken cancellationToken);

    Task<PaymentResponse> MarkPaidAsync(int rentalId, CancellationToken cancellationToken);
    Task<PaymentResponse> MarkPaidForUserAsync(int rentalId, int userId, CancellationToken cancellationToken);
}
