using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Application.Abstractions;
using RentACarAPI.Application.Locations.Contracts;

namespace RentACarAPI.Application.Locations;

public sealed class LocationService : ILocationService
{
    private readonly IAppDbContext _dbContext;

    public LocationService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<LocationResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Locations
            .AsNoTracking()
            .OrderBy(l => l.Id)
            .Select(l => new LocationResponse(l.Id, l.Name, l.City, l.Address, l.Latitude, l.Longitude))
            .ToListAsync(cancellationToken);
    }

    public async Task<LocationResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new LocationResponse(l.Id, l.Name, l.City, l.Address, l.Latitude, l.Longitude))
            .FirstOrDefaultAsync(cancellationToken);

        if (location is null)
        {
            throw new InvalidOperationException("Location not found.");
        }

        return location;
    }

    public async Task<LocationResponse> CreateAsync(CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var location = new Location
        {
            Name = request.Name.Trim(),
            City = request.City.Trim(),
            Address = request.Address.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        _dbContext.Locations.Add(location);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LocationResponse(location.Id, location.Name, location.City, location.Address, location.Latitude, location.Longitude);
    }
}
