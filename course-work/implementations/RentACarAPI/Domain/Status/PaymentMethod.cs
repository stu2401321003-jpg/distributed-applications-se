namespace Domain.Enums;

public static class PaymentMethod
{
    public const string Cash = "Cash";
    public const string Card = "Card";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        Cash,
        Card
    };
}
