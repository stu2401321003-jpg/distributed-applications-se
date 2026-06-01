using RentACarAPI.Application.CarCategories.Contracts;

namespace RentACarAPI.Application.CarCategories;

public interface ICarCategoryService
{
    Task<IReadOnlyCollection<CarCategoryResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<CarCategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<CarCategoryResponse> CreateAsync(CreateCarCategoryRequest request, CancellationToken cancellationToken);
}
