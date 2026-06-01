using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.CarCategories.Contracts;

public sealed record CreateCarCategoryRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    public string Name { get; init; } = null!;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(250, MinimumLength = 2, ErrorMessage = "Description must be between 2 and 250 characters.")]
    public string Description { get; init; } = null!;
}
