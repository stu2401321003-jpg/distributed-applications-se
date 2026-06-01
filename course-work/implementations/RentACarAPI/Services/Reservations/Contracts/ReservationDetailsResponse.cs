namespace RentACarAPI.Application.Reservations.Contracts;

public sealed record ReservationDetailsResponse(
    int Id,
    int UserId,
    int CarId,
    int PickupLocationId,
    int DropoffLocationId,
    DateTime StartDate,
    DateTime EndDate,
    string Status,
    DateTime CreatedAt,
    string CarPlateNumber,
    string CarModel,
    string PickupLocationName,
    string DropoffLocationName);
