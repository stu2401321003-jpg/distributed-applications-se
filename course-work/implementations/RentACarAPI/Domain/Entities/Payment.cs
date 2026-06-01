namespace Domain.Entities;

public sealed class Payment
{
    public int Id { get; set; }

    public int RentalId { get; set; }
    public Rental Rental { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }

    public string Method { get; set; } = null!;

    public string Status { get; set; } = "";
}