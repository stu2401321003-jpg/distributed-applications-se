namespace Domain.Entities;

public sealed class TariffPlan
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal DailyPrice { get; set; }
    public decimal? WeeklyPrice { get; set; }
    public decimal? MonthlyPrice { get; set; }
    public int? MileageLimitPerDay { get; set; }
    public decimal ExtraKmPrice { get; set; }

    public ICollection<Car> Cars { get; set; } = new List<Car>();
}