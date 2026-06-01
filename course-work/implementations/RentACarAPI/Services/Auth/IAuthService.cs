using RentACarAPI.Application.Auth.Contracts;

namespace RentACarAPI.Application.Auth;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
