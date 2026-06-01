using ServiceStack.DataAnnotations;

namespace Domain.Entities;

public sealed class Car
{
    public int Id { get; set; }
    public string PlateNumber { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int Year { get; set; }

    public int CategoryId { get; set; }
    public CarCategory Category { get; set; } = null!;

    public int CurrentLocationId { get; set; }
    public Location CurrentLocation { get; set; } = null!;

    public decimal DailyBasePrice { get; set; }
    public bool IsActive { get; set; }

    public string? ImageUrl { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<TariffPlan> TariffPlans { get; set; } = new List<TariffPlan>();
    public ICollection<User> Users { get; set; } = new List<User>();
}