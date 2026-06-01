namespace Domain.Entities;

public sealed class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string DrivingLicenseNumber { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<Car> Cars { get; set; } = new List<Car>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}