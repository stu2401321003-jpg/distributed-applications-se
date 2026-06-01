using RentACarAPI.Application.ExtraServices.Contracts;

namespace RentACarAPI.Application.ExtraServices;

public interface IExtraServiceService
{
    Task<IReadOnlyCollection<ExtraServiceResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<ExtraServiceResponse> CreateAsync(CreateExtraServiceRequest request, CancellationToken cancellationToken);
}
