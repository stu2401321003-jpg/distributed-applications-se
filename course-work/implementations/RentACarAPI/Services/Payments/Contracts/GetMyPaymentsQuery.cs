namespace RentACarAPI.Application.Payments.Contracts;

public sealed record GetMyPaymentsQuery(
    string? Status,
    DateTime? From,
    DateTime? To);
