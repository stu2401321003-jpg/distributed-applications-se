using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Application.Abstractions;

namespace Infrastructure;

public sealed class CarRentalDbContext : DbContext, IAppDbContext
{
    public CarRentalDbContext(DbContextOptions<CarRentalDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<CarCategory> CarCategories => Set<CarCategory>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Rental> Rentals => Set<Rental>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ExtraService> ExtraServices => Set<ExtraService>();
    public DbSet<RentalExtraService> RentalExtraServices => Set<RentalExtraService>();
    public DbSet<TariffPlan> TariffPlans => Set<TariffPlan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
                .HasMany(c => c.Cars)
                .WithMany(u => u.Users)
                .UsingEntity(
                    "UserFavoriteCars",
                    r => r.HasOne(typeof(Car)).WithMany().HasForeignKey("CarId"),
                    l => l.HasOne(typeof(User)).WithMany().HasForeignKey("UserId"),
                    j => j.HasKey("UserId", "CarId"));


        modelBuilder.Entity<Car>()
              .HasMany(t => t.TariffPlans)
              .WithMany(c => c.Cars)
              .UsingEntity(
                  "VehicleTariffPlans",
                  r => r.HasOne(typeof(TariffPlan)).WithMany().HasForeignKey("TariffPlanId"),
                  l => l.HasOne(typeof(Car)).WithMany().HasForeignKey("CarId"),
                  j => j.HasKey("TariffPlanId", "CarId"));

        modelBuilder.Entity<Reservation>()
                 .HasOne(r => r.PickupLocation)
                 .WithMany(l => l.PickupReservations)
                 .HasForeignKey(r => r.PickupLocationId)
                 .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.DropoffLocation)
            .WithMany(l => l.DropoffReservations)
            .HasForeignKey(r => r.DropoffLocationId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}