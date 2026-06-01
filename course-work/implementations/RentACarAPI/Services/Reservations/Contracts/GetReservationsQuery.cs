namespace RentACarAPI.Application.Reservations.Contracts;

public sealed record GetReservationsQuery(
    string? Status,
    int? UserId,
    int? CarId,
    DateTime? From,
    DateTime? To);
