using RentACarAPI.Application.Locations.Contracts;

namespace RentACarAPI.Application.Locations;

public interface ILocationService
{
    Task<IReadOnlyCollection<LocationResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<LocationResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<LocationResponse> CreateAsync(CreateLocationRequest request, CancellationToken cancellationToken);
}
