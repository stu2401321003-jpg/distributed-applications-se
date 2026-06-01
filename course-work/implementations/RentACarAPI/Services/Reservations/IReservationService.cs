using RentACarAPI.Application.Reservations.Contracts;

namespace RentACarAPI.Application.Reservations;

public interface IReservationService
{
    Task<ReservationResponse> CreateAsync(CreateReservationRequest request, int userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ReservationResponse>> GetMineAsync(int userId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ReservationListItemResponse>> GetAllAsync(GetReservationsQuery query, CancellationToken cancellationToken);
    Task<ReservationDetailsResponse> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ReservationResponse> ApproveAsync(int id, CancellationToken cancellationToken);
    Task<ReservationResponse> RejectAsync(int id, CancellationToken cancellationToken);
    Task CancelAsync(int id, int userId, CancellationToken cancellationToken);
}
