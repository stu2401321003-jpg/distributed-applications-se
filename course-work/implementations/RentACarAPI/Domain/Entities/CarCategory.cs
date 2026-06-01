namespace Domain.Entities;

public sealed class CarCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<Car> Cars { get; set; } = new List<Car>();
}