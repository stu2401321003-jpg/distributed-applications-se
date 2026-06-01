using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.Reservations.Contracts;

public sealed record CreateReservationRequest
{
    [Required(ErrorMessage = "CarId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "CarId must be a positive number.")]
    public int CarId { get; init; }

    [Required(ErrorMessage = "PickupLocationId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "PickupLocationId must be a positive number.")]
    public int PickupLocationId { get; init; }

    [Required(ErrorMessage = "DropoffLocationId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "DropoffLocationId must be a positive number.")]
    public int DropoffLocationId { get; init; }

    [Required(ErrorMessage = "StartDate is required.")]
    public DateTime StartDate { get; init; }

    [Required(ErrorMessage = "EndDate is required.")]
    public DateTime EndDate { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "TariffPlanId must be a positive number.")]
    public int? TariffPlanId { get; init; }
}
