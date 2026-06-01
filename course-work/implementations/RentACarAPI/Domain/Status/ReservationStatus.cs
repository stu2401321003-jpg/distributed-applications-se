namespace Domain.Enums;

public static class ReservationStatus
{
    public const string Pending = "Pending";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";

    public const string InProgress = "InProgress";
    public const string Completed = "Completed";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        Pending,
        Approved,
        Rejected,
        Cancelled,
        InProgress,
        Completed
    };
}
