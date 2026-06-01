namespace RentACarAPI.Application.Locations.Contracts;

public sealed record LocationResponse(
    int Id,
    string Name,
    string City,
    string Address,
    decimal? Latitude,
    decimal? Longitude);
