namespace RentACarAPI.Application.Rentals.Contracts;

public sealed record RentalExtraServiceResponse(
    int ExtraServiceId,
    string Name,
    decimal PricePerDay,
    int Quantity);
