using Skeleton.Application.Feature.Category.CategoryDto;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.ImplementationServices;

public class CategoryService : ICategoryService
{
    private readonly IReadRepository<Category> _readRepository;
    private readonly IWriteRepository<Category> _writeRepository;
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly CategoryBusinessRules _businessRules;
    private readonly IMapper _mapper;

    // Constructure Category 
    public CategoryService(
        IReadRepository<Category> readRepository,
        IWriteRepository<Category> writeRepository,
        ICategoryQueryRepository queryRepository,
        CategoryBusinessRules businessRules,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _queryRepository = queryRepository;
        _businessRules = businessRules;
        _mapper = mapper;
    }

    // =======================
    // Queries
    // =======================
    public async Task<PagedResult<CategoryResponseDto>> GetPagedCategoryAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0) pageNumber = PaginationDefaults.PageNumber;
        if (pageSize <= 0) pageSize = PaginationDefaults.PageSize;

        var categories = await _queryRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);

        return categories;
    }

    public async Task<IReadOnlyList<CategoryResponseDto>> GetAllCategoriesAsync(CancellationToken cancellationToken)
        => await _queryRepository.GetAllCategoriesAsync(cancellationToken);

    public async Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _queryRepository.GetCategoryByIdAsync(id, cancellationToken);

        DtoBusinessRules.EnsureExists(category, id, nameof(Category));

        return category;
    }

    public Task<IReadOnlyList<CategoryResponseDto>> SearchCategoriesAsync(string keyword, CancellationToken cancellationToken)
    => _queryRepository.SearchAsync(keyword, cancellationToken);

    // =======================
    // Writes
    // =======================

    public async Task<CategoryResponseDto> AddCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        await _businessRules.EnsureUniqueAsync(c => c.Name == dto.Name,
            "Category name already exists.", cancellationToken);

        var category = new Category(dto.Name, dto.Description);

        await _writeRepository.AddAsync(category);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto> UpdateCategoryAsync(UpdateCategoryDto dto, CancellationToken cancellationToken)
    {
        var category = await _readRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (category is null)
            throw new BusinessException(ErrorType.NotFound, entityName: nameof(Category), entityId: dto.Id);

        await _businessRules.EnsureUniqueAsync(c => c.Name == dto.Name && c.Id != dto.Id,
            "Category already exists.", cancellationToken);

        category.Update(dto.Name, dto.Description);

        await _writeRepository.UpdateAsync(category);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _readRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new BusinessException(ErrorType.NotFound, entityName: nameof(Category), entityId: id);

        await _businessRules.EnsureCategoryHasNoProducts(id, cancellationToken);

        await _writeRepository.DeleteAsync(category);

        await _writeRepository.SaveChangesAsync(cancellationToken);
    }
}
