namespace RentACarAPI.Application.Rentals.Contracts;

public sealed record RentalResponse(
    int Id,
    int ReservationId,
    DateTime StartActual,
    DateTime? EndActual,
    int StartMileage,
    int? EndMileage,
    string Status,
    decimal TotalPrice);
