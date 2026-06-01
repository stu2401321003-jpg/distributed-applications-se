using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.Auth.Contracts;

public sealed record LoginRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not valid.")]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; init; } = null!;
}
