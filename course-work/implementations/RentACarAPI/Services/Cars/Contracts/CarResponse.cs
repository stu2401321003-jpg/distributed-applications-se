namespace RentACarAPI.Application.Cars.Contracts;

public sealed record CarResponse(
    int Id,
    string PlateNumber,
    string Model,
    int Year,
    int CategoryId,
    int CurrentLocationId,
    decimal DailyBasePrice,
    bool IsActive,
    string? ImageUrl);
