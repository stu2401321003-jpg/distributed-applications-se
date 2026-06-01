namespace Domain.Enums;

public static class PaymentStatus
{
    public const string Pending = "Pending";
    public const string Paid = "Paid";
    public const string Failed = "Failed";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        Pending,
        Paid,
        Failed,
        Cancelled
    };
}
