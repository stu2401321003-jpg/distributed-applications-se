using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.ExtraServices.Contracts;

public sealed record CreateExtraServiceRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; init; } = null!;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(250, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 250 characters.")]
    public string Description { get; init; } = null!;

    [Range(typeof(decimal), "0.0", "9999999", ErrorMessage = "PricePerDay must be non-negative.")]
    public decimal PricePerDay { get; init; }
}
