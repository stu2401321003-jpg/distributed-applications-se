using RentACarAPI.Application.Cars;

namespace RentACarAPI.Infrastructure.Storage;

public sealed class LocalCarImageStorage : ICarImageStorage
{
    private readonly string _webRootPath;

    public LocalCarImageStorage(IWebHostEnvironment env)
    {
        _webRootPath = env.WebRootPath;
    }

    public async Task<string> SaveAsync(int carId, Stream content, string fileExtension, CancellationToken cancellationToken)
    {
        var safeExt = fileExtension.StartsWith('.') ? fileExtension : $".{fileExtension}";
        var fileName = $"{Guid.NewGuid():N}{safeExt}";

        var folder = Path.Combine(_webRootPath, "uploads", "cars", carId.ToString());
        Directory.CreateDirectory(folder);

        var fullPath = Path.Combine(folder, fileName);
        await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(fs, cancellationToken);

        // public relative url (served by UseStaticFiles)
        return $"/uploads/cars/{carId}/{fileName}";
    }
}
