
using Skeleton.Domain.Entities;

namespace Skeleton.Domain.Interfaces.InterfacesRepository;

public interface IWriteRepository<TEntity> where TEntity : BaseEntity
{
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
