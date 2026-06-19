using Skeleton.Application.Feature.ProductReview.ReviewDto;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.ImplementationServices;

// ════════════════════════════════════════
//  ProductReview Service
// ════════════════════════════════════════
public class ProductReviewService : IProductReviewService
{
    private readonly IReadRepository<Domain.Entities.ProductReview> _readRepository;
    private readonly IWriteRepository<Domain.Entities.ProductReview> _writeRepository;
    private readonly IReadRepository<Domain.Entities.Product> _productReadRepository;
    private readonly IProductReviewQueryRepository _queryRepository;
    private readonly IMapper _mapper;

    public ProductReviewService(
        IReadRepository<Domain.Entities.ProductReview> readRepository,
        IWriteRepository<Domain.Entities.ProductReview> writeRepository,
        IReadRepository<Domain.Entities.Product> productReadRepository,
        IProductReviewQueryRepository queryRepository,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _productReadRepository = productReadRepository;
        _queryRepository = queryRepository;
        _mapper = mapper;
    }

    public async Task<ReviewResponseDto> CreateReviewAsync(CreateReviewDto dto, CancellationToken cancellationToken)
    {
        var product = await _productReadRepository.GetByIdAsync(dto.ProductId, cancellationToken);
        DtoBusinessRules.EnsureExists(product, dto.ProductId, nameof(Domain.Entities.Product));

        var alreadyReviewed = await _readRepository.AnyAsync(
            r => r.ProductId == dto.ProductId && r.CustomerId == dto.CustomerId, cancellationToken);
        if (alreadyReviewed)
            throw new BusinessException(ErrorType.Conflict, "You have already reviewed this product.");

        var review = new Domain.Entities.ProductReview(dto.ProductId, dto.CustomerId, dto.Rating, dto.Title, dto.Body);
        await _writeRepository.AddAsync(review);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ReviewResponseDto>(review);
    }

    public async Task DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        var review = await _readRepository.GetByIdAsync(reviewId, cancellationToken);
        DtoBusinessRules.EnsureExists(review, reviewId, nameof(Domain.Entities.ProductReview));

        await _writeRepository.DeleteAsync(review!);
        await _writeRepository.SaveChangesAsync(cancellationToken);
    }

    public Task<IReadOnlyList<ReviewResponseDto>> GetReviewsByProductIdAsync(Guid productId, CancellationToken cancellationToken)
        => _queryRepository.GetReviewsByProductIdAsync(productId, cancellationToken);
}
