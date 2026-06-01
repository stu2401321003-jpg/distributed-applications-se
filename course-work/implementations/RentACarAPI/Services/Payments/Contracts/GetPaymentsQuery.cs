namespace RentACarAPI.Application.Payments.Contracts;

public sealed record GetPaymentsQuery(
    string? Status,
    string? Method,
    int? UserId,
    int? RentalId,
    DateTime? From,
    DateTime? To);
