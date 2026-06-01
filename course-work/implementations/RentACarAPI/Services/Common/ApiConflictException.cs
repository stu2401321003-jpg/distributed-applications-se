namespace RentACarAPI.Application.Common;

public sealed class ApiConflictException : Exception
{
    public ApiConflictException(string message)
        : base(message)
    {
    }
}
