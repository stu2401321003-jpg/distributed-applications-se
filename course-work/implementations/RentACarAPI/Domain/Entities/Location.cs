namespace Domain.Entities;

public sealed class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Address { get; set; } = null!;

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public ICollection<Car> Cars { get; set; } = new List<Car>();
    public ICollection<Reservation> PickupReservations { get; set; } = new List<Reservation>();
    public ICollection<Reservation> DropoffReservations { get; set; } = new List<Reservation>();
}