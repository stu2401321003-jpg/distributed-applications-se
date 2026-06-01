using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.Auth.Contracts;

public sealed record RegisterRequest
{
    [Required(ErrorMessage = "Full name is required.")]
    public string FullName { get; init; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(16, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string Password { get; init; } = null!;

    [Required(ErrorMessage = "Phone is required.")]
    [Phone(ErrorMessage = "Phone is not valid.")]
    public string Phone { get; init; } = null!;

    [Required(ErrorMessage = "Driving license number is required.")]
    [StringLength(20, ErrorMessage = "Driving license number cannot exceed 20 characters.")]
    public string DrivingLicenseNumber { get; init; } = null!;
}
