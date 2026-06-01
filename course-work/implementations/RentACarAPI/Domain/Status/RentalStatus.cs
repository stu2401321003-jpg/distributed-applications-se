namespace Domain.Enums;

public static class RentalStatus
{
    public const string Open = "Open";
    public const string Closed = "Closed";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        Open,
        Closed,
        Cancelled
    };
}
