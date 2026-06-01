using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Application.Abstractions;

namespace RentACarAPI.Infrastructure.Seeding;

public sealed class DatabaseSeeder
{
    private readonly IAppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(IAppDbContext dbContext, IConfiguration configuration, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await EnsureRoleAsync("Admin", cancellationToken);
        await EnsureRoleAsync("Employee", cancellationToken);
        await EnsureRoleAsync("Customer", cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await EnsureAdminUserAsync(cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await SeedDemoDataAsync(cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Roles.AnyAsync(r => r.Name == roleName, cancellationToken);
        if (exists)
        {
            return;
        }

        _dbContext.Roles.Add(new Role { Name = roleName });
    }

    private async Task EnsureAdminUserAsync(CancellationToken cancellationToken)
    {
        var section = _configuration.GetSection("SeedAdmin");

        var email = section["Email"];
        var password = section["Password"];
        var fullName = section["FullName"];
        var phone = section["Phone"];
        var drivingLicenseNumber = section["DrivingLicenseNumber"];

        var adminUser = await _dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (adminUser is null)
        {
            adminUser = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = _passwordHasher.Hash(password),
                Phone = phone,
                DrivingLicenseNumber = drivingLicenseNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _dbContext.Users.Add(adminUser);
        }

        var adminRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Admin", cancellationToken);
            adminUser.Roles.Add(adminRole);
    }

    private async Task SeedDemoDataAsync(CancellationToken cancellationToken)
    {
        var sofia = await EnsureLocationAsync("Sofia Center", "Sofia", "1 Vitosha Blvd", cancellationToken);
        var plovdiv = await EnsureLocationAsync("Plovdiv Office", "Plovdiv", "10 Main St", cancellationToken);

        var economy = await EnsureCategoryAsync("Economy", "Small, fuel efficient cars.", cancellationToken);
        var suv = await EnsureCategoryAsync("SUV", "Spacious SUVs for family trips.", cancellationToken);

        await EnsureCarAsync("CA0001AA", "Toyota Yaris", 2022, economy.Id, sofia.Id, 45m, cancellationToken);
        await EnsureCarAsync("PB0002BB", "Dacia Duster", 2023, suv.Id, plovdiv.Id, 80m, cancellationToken);
    }

    private async Task<Location> EnsureLocationAsync(string name, string city, string address, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.Name == name, cancellationToken);
        if (location is not null)
        {
            return location;
        }

        location = new Location
        {
            Name = name,
            City = city,
            Address = address
        };

        _dbContext.Locations.Add(location);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return location;
    }

    private async Task<CarCategory> EnsureCategoryAsync(string name, string description, CancellationToken cancellationToken)
    {
        var category = await _dbContext.CarCategories.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
        if (category is not null)
        {
            return category;
        }

        category = new CarCategory
        {
            Name = name,
            Description = description
        };

        _dbContext.CarCategories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return category;
    }

    private async Task<Car> EnsureCarAsync(
        string plateNumber,
        string model,
        int year,
        int categoryId,
        int locationId,
        decimal dailyBasePrice,
        CancellationToken cancellationToken)
    {
        var car = await _dbContext.Cars.FirstOrDefaultAsync(c => c.PlateNumber == plateNumber, cancellationToken);
        if (car is not null)
        {
            return car;
        }

        car = new Car
        {
            PlateNumber = plateNumber,
            Model = model,
            Year = year,
            CategoryId = categoryId,
            CurrentLocationId = locationId,
            DailyBasePrice = dailyBasePrice,
            IsActive = true
        };

        _dbContext.Cars.Add(car);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return car;
    }
}
