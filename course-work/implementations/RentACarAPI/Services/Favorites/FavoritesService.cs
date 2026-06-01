using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Application.Abstractions;
using RentACarAPI.Application.Cars.Contracts;

namespace RentACarAPI.Application.Favorites;

public sealed class FavoritesService : IFavoritesService
{
    private readonly IAppDbContext _dbContext;

    public FavoritesService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<CarResponse>> GetMineAsync(int userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Cars)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Id)
            .Select(c => new CarResponse(
                c.Id,
                c.PlateNumber,
                c.Model,
                c.Year,
                c.CategoryId,
                c.CurrentLocationId,
                c.DailyBasePrice,
                c.IsActive,
                c.ImageUrl))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(int userId, int carId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.Cars)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var car = await _dbContext.Cars
            .FirstOrDefaultAsync(c => c.Id == carId, cancellationToken);

        if (car is null)
        {
            throw new InvalidOperationException("Car not found.");
        }

        if (!car.IsActive)
        {
            throw new InvalidOperationException("Inactive cars cannot be favorited.");
        }

        if (user.Cars.Any(c => c.Id == carId))
        {
            return;
        }

        user.Cars.Add(car);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(int userId, int carId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.Cars)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var existing = user.Cars.FirstOrDefault(c => c.Id == carId);
        if (existing is null)
        {
            return;
        }

        user.Cars.Remove(existing);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
