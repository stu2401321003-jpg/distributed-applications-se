using System.ComponentModel.DataAnnotations;

namespace RentACarAPI.Application.TariffPlans.Contracts;

public sealed record CreateTariffPlanRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; init; } = null!;

    [Range(typeof(decimal), "0.0", "9999999", ErrorMessage = "DailyPrice must be non-negative.")]
    public decimal DailyPrice { get; init; }

    [Range(typeof(decimal), "0.0", "9999999", ErrorMessage = "WeeklyPrice must be non-negative.")]
    public decimal? WeeklyPrice { get; init; }

    [Range(typeof(decimal), "0.0", "9999999", ErrorMessage = "MonthlyPrice must be non-negative.")]
    public decimal? MonthlyPrice { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "MileageLimitPerDay must be non-negative.")]
    public int? MileageLimitPerDay { get; init; }

    [Range(typeof(decimal), "0.0", "9999999", ErrorMessage = "ExtraKmPrice must be non-negative.")]
    public decimal ExtraKmPrice { get; init; }
}
