using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.Rentals.Contracts;

public sealed record CloseRentalRequest
{
    public DateTime? EndActual { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "EndMileage must be a non-negative number.")]
    public int EndMileage { get; init; }
}
