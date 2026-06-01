using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public sealed class Reservation
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int CarId { get; set; }
    public Car Car { get; set; } = null!;

    public int PickupLocationId { get; set; }
    [ForeignKey("PickupLocationId")]
    [InverseProperty("PickupReservations")]
    public Location PickupLocation { get; set; } = null!;

    public int DropoffLocationId { get; set; }
    [ForeignKey("DropoffLocationId")]
    [InverseProperty("DropoffReservations")]
    public Location DropoffLocation { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int? TariffPlanId { get; set; }
    public TariffPlan? TariffPlan { get; set; }

    public string Status { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    public Rental Rental { get; set; } = null!;
}