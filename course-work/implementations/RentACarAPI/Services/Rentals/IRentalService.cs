using RentACarAPI.Application.Rentals.Contracts;

namespace RentACarAPI.Application.Rentals;

public interface IRentalService
{
    Task<RentalResponse> CreateFromReservationAsync(CreateRentalRequest request, CancellationToken cancellationToken);
    Task<RentalResponse> CloseAsync(int rentalId, CloseRentalRequest request, CancellationToken cancellationToken);
    Task<RentalResponse> GetByIdAsync(int rentalId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RentalResponse>> GetMineAsync(int userId, CancellationToken cancellationToken);
    Task<RentalResponse> GetMineByIdAsync(int rentalId, int userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RentalResponse>> GetAllAsync(GetRentalsQuery query, CancellationToken cancellationToken);

    Task<RentalDetailsResponse> GetDetailsAsync(int rentalId, CancellationToken cancellationToken);
    Task<RentalDetailsResponse> GetMyDetailsAsync(int rentalId, int userId, CancellationToken cancellationToken);
    Task<RentalDetailsResponse> AddExtraAsync(int rentalId, AddRentalExtraServiceRequest request, CancellationToken cancellationToken);
    Task<RentalDetailsResponse> AddExtraForUserAsync(int rentalId, int userId, AddRentalExtraServiceRequest request, CancellationToken cancellationToken);
    Task<RentalDetailsResponse> RemoveExtraAsync(int rentalId, int extraServiceId, CancellationToken cancellationToken);
    Task<RentalDetailsResponse> RemoveExtraForUserAsync(int rentalId, int userId, int extraServiceId, CancellationToken cancellationToken);
}
