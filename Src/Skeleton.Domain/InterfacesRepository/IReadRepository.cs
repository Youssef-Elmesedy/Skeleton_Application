using Skeleton.Domain.Entities;
using System.Linq.Expressions;

namespace Skeleton.Domain.Interfaces.InterfacesRepository;

public interface IReadRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    Task<IReadOnlyList<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
}
