namespace RentACarAPI.Application.CurrentUser;

public interface ICurrentUserService
{
    int? GetUserId();
    int GetUserIdOrThrow();
}
