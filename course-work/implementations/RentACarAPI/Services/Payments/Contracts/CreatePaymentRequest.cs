using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.Payments.Contracts;

public sealed record CreatePaymentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "RentalId must be a positive number.")]
    public int RentalId { get; init; }

    [Required(ErrorMessage = "Method is required.")]
    public string Method { get; init; } = null!;
}
