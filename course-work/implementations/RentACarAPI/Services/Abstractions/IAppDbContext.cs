using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace RentACarAPI.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Location> Locations { get; }
    DbSet<CarCategory> CarCategories { get; }
    DbSet<Car> Cars { get; }
    DbSet<Reservation> Reservations { get; }
    DbSet<Rental> Rentals { get; }
    DbSet<Payment> Payments { get; }
    DbSet<ExtraService> ExtraServices { get; }
    DbSet<RentalExtraService> RentalExtraServices { get; }
    DbSet<TariffPlan> TariffPlans { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
