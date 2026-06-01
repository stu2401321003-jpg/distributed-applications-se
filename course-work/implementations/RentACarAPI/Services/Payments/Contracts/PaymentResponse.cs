namespace RentACarAPI.Application.Payments.Contracts;

public sealed record PaymentResponse(
    int Id,
    int RentalId,
    decimal Amount,
    DateTime CreatedAt,
    string Method,
    string Status);
