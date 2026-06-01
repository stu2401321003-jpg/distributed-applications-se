using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.Rentals.Contracts;

public sealed record AddRentalExtraServiceRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "ExtraServiceId must be a positive number.")]
    public int ExtraServiceId { get; init; }

    [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000.")]
    public int Quantity { get; init; }
}
