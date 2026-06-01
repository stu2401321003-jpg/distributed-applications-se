using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using RentACarAPI.Application.Abstractions;
using RentACarAPI.Application.ExtraServices.Contracts;

namespace RentACarAPI.Application.ExtraServices;

public sealed class ExtraServiceService : IExtraServiceService
{
    private readonly IAppDbContext _dbContext;

    public ExtraServiceService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ExtraServiceResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ExtraServices
            .AsNoTracking()
            .OrderBy(es => es.Id)
            .Select(es => new ExtraServiceResponse(es.Id, es.Name, es.Description, es.PricePerDay))
            .ToListAsync(cancellationToken);
    }

    public async Task<ExtraServiceResponse> CreateAsync(CreateExtraServiceRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var exists = await _dbContext.ExtraServices.AnyAsync(es => es.Name == name, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("Extra service with the same name already exists.");
        }

        var entity = new ExtraService
        {
            Name = name,
            Description = request.Description.Trim(),
            PricePerDay = request.PricePerDay
        };

        _dbContext.ExtraServices.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ExtraServiceResponse(entity.Id, entity.Name, entity.Description, entity.PricePerDay);
    }
}
