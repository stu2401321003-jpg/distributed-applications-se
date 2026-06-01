using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.Cars.Contracts;

public sealed record CreateCarRequest
{
    [Required(ErrorMessage = "PlateNumber is required.")]
    [StringLength(32, MinimumLength = 2, ErrorMessage = "PlateNumber must be between 2 and 32 characters.")]
    public string PlateNumber { get; init; } = null!;

    [Required(ErrorMessage = "Model is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Model must be between 2 and 100 characters.")]
    public string Model { get; init; } = null!;

    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
    public int Year { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a positive number.")]
    public int CategoryId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "CurrentLocationId must be a positive number.")]
    public int CurrentLocationId { get; init; }

    [Range(typeof(decimal), "0.01", "9999999", ErrorMessage = "DailyBasePrice must be positive.")]
    public decimal DailyBasePrice { get; init; }

    [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
    public string? ImageUrl { get; init; }
}
