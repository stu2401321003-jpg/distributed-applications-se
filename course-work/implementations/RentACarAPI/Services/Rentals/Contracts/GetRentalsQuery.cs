namespace RentACarAPI.Application.Rentals.Contracts;

public sealed record GetRentalsQuery(
    string? Status,
    int? UserId,
    int? ReservationId,
    DateTime? From,
    DateTime? To);
