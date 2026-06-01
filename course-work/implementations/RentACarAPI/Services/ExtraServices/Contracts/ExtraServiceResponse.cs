namespace RentACarAPI.Application.ExtraServices.Contracts;

public sealed record ExtraServiceResponse(int Id, string Name, string Description, decimal PricePerDay);
