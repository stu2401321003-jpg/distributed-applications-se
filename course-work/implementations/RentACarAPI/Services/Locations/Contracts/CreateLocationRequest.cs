using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.Locations.Contracts;

public sealed record CreateLocationRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; init; } = null!;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters.")]
    public string City { get; init; } = null!;

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 200 characters.")]
    public string Address { get; init; } = null!;

    [Range(typeof(decimal), "-90", "90", ErrorMessage = "Latitude must be between -90 and 90.")]
    public decimal? Latitude { get; init; }

    [Range(typeof(decimal), "-180", "180", ErrorMessage = "Longitude must be between -180 and 180.")]
    public decimal? Longitude { get; init; }
}
