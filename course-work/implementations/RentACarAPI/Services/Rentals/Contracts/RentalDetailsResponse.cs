namespace RentACarAPI.Application.Rentals.Contracts;

public sealed record RentalDetailsResponse(
    int Id,
    int ReservationId,
    DateTime StartActual,
    DateTime? EndActual,
    int StartMileage,
    int? EndMileage,
    string Status,
    decimal BaseTotalPrice,
    decimal ExtrasTotalPrice,
    decimal TotalPrice,
    IReadOnlyCollection<RentalExtraServiceResponse> Extras);
