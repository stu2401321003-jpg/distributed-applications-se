namespace RentACarAPI.Application.Reservations.Contracts;

public sealed record ReservationResponse(
    int Id,
    int UserId,
    int CarId,
    int PickupLocationId,
    int DropoffLocationId,
    DateTime StartDate,
    DateTime EndDate,
    string Status,
    DateTime CreatedAt);
