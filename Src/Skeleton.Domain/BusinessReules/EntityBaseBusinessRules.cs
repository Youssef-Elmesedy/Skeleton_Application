using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;
using Skeleton.Domain.Interfaces.InterfacesRepository;
using System.Linq.Expressions;

namespace Skeleton.Domain.BusinessRules;

public class EntityBaseBusinessRules<TEntity> where TEntity : BaseEntity
{
    private readonly IReadRepository<TEntity> _readRepository;

    public EntityBaseBusinessRules(IReadRepository<TEntity> readRepository)
    {
        _readRepository = readRepository;
    }

    public TEntity EnsureExists(TEntity? entity, Guid id, string entityName)
    {
        if (entity is null)
            throw new BusinessException(
                ErrorType.NotFound,
                entityName: entityName,
                entityId: id
            );

        return entity;
    }

    public async Task EnsureUniqueAsync(
    Expression<Func<TEntity, bool>> predicate,
    string errorMessage,
    CancellationToken cancellationToken)
    {
        var exists = await _readRepository.AnyAsync(predicate, cancellationToken);

        if (exists)
            throw new BusinessException(ErrorType.Conflict, errorMessage);
    }
}