namespace Domain.Entities;

public sealed class ExtraService
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal PricePerDay { get; set; }

    public ICollection<RentalExtraService> RentalExtraServices { get; set; } = new List<RentalExtraService>();
}