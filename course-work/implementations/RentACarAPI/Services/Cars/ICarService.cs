using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using RentACarAPI.Application.Cars.Contracts;

namespace RentACarAPI.Application.Cars;

public interface ICarService
{
    Task<IReadOnlyCollection<CarResponse>> GetAllAsync(int? categoryId, int? locationId, bool? isActive, CancellationToken cancellationToken);
    Task<CarResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<CarResponse> CreateAsync(CreateCarRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<CarResponse>> GetAvailableAsync(DateTime from, DateTime to, int pickupLocationId, int? categoryId, CancellationToken cancellationToken);

    Task<CarResponse> UploadImageAsync(int carId, IFormFile image, HttpRequest request, CancellationToken cancellationToken);
}
