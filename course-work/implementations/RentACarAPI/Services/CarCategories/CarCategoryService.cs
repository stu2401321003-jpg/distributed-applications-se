using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Application.Abstractions;
using RentACarAPI.Application.CarCategories.Contracts;

namespace RentACarAPI.Application.CarCategories;

public sealed class CarCategoryService : ICarCategoryService
{
    private readonly IAppDbContext _dbContext;

    public CarCategoryService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<CarCategoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.CarCategories
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CarCategoryResponse(c.Id, c.Name, c.Description))
            .ToListAsync(cancellationToken);
    }

    public async Task<CarCategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _dbContext.CarCategories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CarCategoryResponse(c.Id, c.Name, c.Description))
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            throw new InvalidOperationException("Car category not found.");
        }

        return category;
    }

    public async Task<CarCategoryResponse> CreateAsync(CreateCarCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = new CarCategory
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim()
        };

        _dbContext.CarCategories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CarCategoryResponse(category.Id, category.Name, category.Description);
    }
}
