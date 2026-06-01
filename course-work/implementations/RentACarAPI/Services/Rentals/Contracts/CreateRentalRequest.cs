using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.Rentals.Contracts;

public sealed record CreateRentalRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "ReservationId must be a positive number.")]
    public int ReservationId { get; init; }

    public DateTime? StartActual { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "StartMileage must be a non-negative number.")]
    public int StartMileage { get; init; }
}
