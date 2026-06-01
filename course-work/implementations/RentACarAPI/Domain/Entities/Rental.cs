namespace Domain.Entities;

public sealed class Rental
{
    public int Id { get; set; }

    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;

    public DateTime StartActual { get; set; }
    public DateTime? EndActual { get; set; }
    public int StartMileage { get; set; }
    public int? EndMileage { get; set; }

    public string Status { get; set; } = "";

    public decimal TotalPrice { get; set; }

    public Payment Payment { get; set; } = null!;
    public ICollection<RentalExtraService> RentalExtraServices { get; set; } = new List<RentalExtraService>();
}