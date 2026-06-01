namespace RentACarAPI.Application.Cars;

public interface ICarImageStorage
{
    Task<string> SaveAsync(int carId, Stream content, string fileExtension, CancellationToken cancellationToken);
}
