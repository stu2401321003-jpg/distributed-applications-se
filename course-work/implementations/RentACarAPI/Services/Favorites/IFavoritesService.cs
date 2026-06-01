using RentACarAPI.Application.Cars.Contracts;

namespace RentACarAPI.Application.Favorites;

public interface IFavoritesService
{
    Task<IReadOnlyCollection<CarResponse>> GetMineAsync(int userId, CancellationToken cancellationToken);
    Task AddAsync(int userId, int carId, CancellationToken cancellationToken);
    Task RemoveAsync(int userId, int carId, CancellationToken cancellationToken);
}
