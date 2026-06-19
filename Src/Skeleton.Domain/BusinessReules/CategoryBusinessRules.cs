using Skeleton.Domain.BusinessRules;
using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Domain.BusinessReules;

public class CategoryBusinessRules : EntityBaseBusinessRules<Category>
{
    private readonly IReadRepository<Product> _productRepository;

    public CategoryBusinessRules(
        IReadRepository<Category> categoryRepository,
        IReadRepository<Product> productRepository
    ) : base(categoryRepository)
    {
        _productRepository = productRepository;
    }

    public async Task EnsureCategoryHasNoProducts(Guid categoryId, CancellationToken cancellationToken)
    {
        var exist = await _productRepository.AnyAsync(p => p.CategoryId == categoryId, cancellationToken);
        if (exist)
            throw new BusinessException(
                ErrorType.Validation,
                "Category has products and cannot be deleted",
                nameof(Category)
            );
    }
}
