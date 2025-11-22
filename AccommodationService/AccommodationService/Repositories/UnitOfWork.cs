using AccommodationService.Data;
using AccommodationService.Repositories.Interfaces;

namespace AccommodationService.Repositories;

public class UnitOfWork(
    ApplicationDbContext context)
    : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}