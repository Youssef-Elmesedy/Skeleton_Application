using Skeleton.Domain.BusinessRules;
using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Domain.BusinessReules;

public class ProductBusinessRules : EntityBaseBusinessRules<Product>
{
    private readonly IReadRepository<Category> _categoryReadRepository;

    public ProductBusinessRules(
        IReadRepository<Product> productRepository,
        IReadRepository<Category> categoryReadRepository
    ) : base(productRepository)
    {
        _categoryReadRepository = categoryReadRepository;
    }

    public async Task EnsureCategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        if (!await _categoryReadRepository.AnyAsync(c => c.Id == categoryId, cancellationToken))
            throw new BusinessException(
                ErrorType.NotFound,
                "Category not found",
                entityName: nameof(Category),
                entityId: categoryId
            );
    }
}