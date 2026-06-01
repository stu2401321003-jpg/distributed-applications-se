namespace RentACarAPI.Application.TariffPlans.Contracts;

public sealed record TariffPlanResponse(
    int Id,
    string Name,
    decimal DailyPrice,
    decimal? WeeklyPrice,
    decimal? MonthlyPrice,
    int? MileageLimitPerDay,
    decimal ExtraKmPrice);
