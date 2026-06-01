using Domain.Entities;

namespace RentACarAPI.Application.Rentals.Pricing;

public static class TariffPricing
{
    public static decimal CalculateBaseTotal(decimal days, decimal dailyFallbackPrice, TariffPlan? tariffPlan)
    {
        if (tariffPlan is null)
        {
            return days * dailyFallbackPrice;
        }

        // Minimal implementation (A+B): if a plan is selected we apply its DailyPrice as base.
        // Weekly/Monthly can be introduced later when you decide on the pricing rules.
        return days * tariffPlan.DailyPrice;
    }
}
