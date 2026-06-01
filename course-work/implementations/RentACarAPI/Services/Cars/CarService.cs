using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Application.Abstractions;
using RentACarAPI.Application.Cars.Contracts;

namespace RentACarAPI.Application.Cars;

public sealed class CarService : ICarService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICarImageStorage _imageStorage;

    public CarService(IAppDbContext dbContext, ICarImageStorage imageStorage)
    {
        _dbContext = dbContext;
        _imageStorage = imageStorage;
    }

    public async Task<IReadOnlyCollection<CarResponse>> GetAllAsync(int? categoryId, int? locationId, bool? isActive, CancellationToken cancellationToken)
    {
        IQueryable<Car> query = _dbContext.Cars.AsNoTracking();

        if (categoryId is not null)
        {
            query = query.Where(c => c.CategoryId == categoryId.Value);
        }

        if (locationId is not null)
        {
            query = query.Where(c => c.CurrentLocationId == locationId.Value);
        }

        if (isActive is not null)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        return await query
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

    public async Task<CarResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var car = await _dbContext.Cars
            .AsNoTracking()
            .Where(c => c.Id == id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (car is null)
        {
            throw new InvalidOperationException("Car not found.");
        }

        return car;
    }

    public async Task<CarResponse> CreateAsync(CreateCarRequest request, CancellationToken cancellationToken)
    {
        var categoryExists = await _dbContext.CarCategories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new InvalidOperationException("Car category not found.");
        }

        var locationExists = await _dbContext.Locations.AnyAsync(l => l.Id == request.CurrentLocationId, cancellationToken);
        if (!locationExists)
        {
            throw new InvalidOperationException("Location not found.");
        }

        var plate = request.PlateNumber.Trim();
        var plateExists = await _dbContext.Cars.AnyAsync(c => c.PlateNumber == plate, cancellationToken);
        if (plateExists)
        {
            throw new InvalidOperationException("Car with the same plate number already exists.");
        }

        var car = new Car
        {
            PlateNumber = plate,
            Model = request.Model.Trim(),
            Year = request.Year,
            CategoryId = request.CategoryId,
            CurrentLocationId = request.CurrentLocationId,
            DailyBasePrice = request.DailyBasePrice,
            IsActive = true,
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim()
        };

        _dbContext.Cars.Add(car);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CarResponse(
            car.Id,
            car.PlateNumber,
            car.Model,
            car.Year,
            car.CategoryId,
            car.CurrentLocationId,
            car.DailyBasePrice,
            car.IsActive,
            car.ImageUrl);
    }

    public async Task<IReadOnlyCollection<CarResponse>> GetAvailableAsync(DateTime from, DateTime to, int pickupLocationId, int? categoryId, CancellationToken cancellationToken)
    {
        if (to <= from)
        {
            throw new InvalidOperationException("The 'to' date must be after the 'from' date.");
        }

        var locationExists = await _dbContext.Locations.AnyAsync(l => l.Id == pickupLocationId, cancellationToken);
        if (!locationExists)
        {
            throw new InvalidOperationException("Pickup location not found.");
        }

        IQueryable<Car> query = _dbContext.Cars
            .AsNoTracking()
            .Where(c => c.IsActive)
            .Where(c => c.CurrentLocationId == pickupLocationId);

        if (categoryId is not null)
        {
            query = query.Where(c => c.CategoryId == categoryId.Value);
        }

        return await query
            .Where(c => !_dbContext.Reservations.Any(r =>
                r.CarId == c.Id &&
                r.Status != ReservationStatus.Cancelled &&
                r.Status != ReservationStatus.Rejected &&
                from < r.EndDate &&
                to > r.StartDate))
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

    public async Task<CarResponse> UploadImageAsync(int carId, IFormFile image, HttpRequest request, CancellationToken cancellationToken)
    {
        if (image.Length <= 0)
        {
            throw new InvalidOperationException("Empty file.");
        }

        var ext = Path.GetExtension(image.FileName);
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowed.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Unsupported image type. Allowed: jpg, jpeg, png, webp.");
        }

        var car = await _dbContext.Cars.FirstOrDefaultAsync(c => c.Id == carId, cancellationToken);
        if (car is null)
        {
            throw new InvalidOperationException("Car not found.");
        }

        await using var stream = image.OpenReadStream();
        var relativeUrl = await _imageStorage.SaveAsync(carId, stream, ext, cancellationToken);

        // store absolute URL to simplify UI
        var baseUrl = $"{request.Scheme}://{request.Host}";
        car.ImageUrl = $"{baseUrl}{relativeUrl}";

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CarResponse(
            car.Id,
            car.PlateNumber,
            car.Model,
            car.Year,
            car.CategoryId,
            car.CurrentLocationId,
            car.DailyBasePrice,
            car.IsActive,
            car.ImageUrl);
    }
}
