using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

[PrimaryKey("RentalId", "ExtraServiceId")]
public sealed class RentalExtraService
{
    public int RentalId { get; set; }
    public Rental Rental { get; set; } = null!;

    public int ExtraServiceId { get; set; }
    public ExtraService ExtraService { get; set; } = null!;

    public int Quantity { get; set; }
}