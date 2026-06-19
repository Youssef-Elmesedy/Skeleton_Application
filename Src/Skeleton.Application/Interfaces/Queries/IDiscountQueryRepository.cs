using Skeleton.Application.Feature.Discount.DiscountDto;

namespace Skeleton.Application.Interfaces.Queries;

public interface IDiscountQueryRepository
{
    Task<DiscountResponseDto?> GetDiscountByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<DiscountResponseDto>> GetAllDiscountsAsync(CancellationToken cancellationToken);
    Task<DiscountResponseDto?> GetByCodeAsync(string code, CancellationToken cancellationToken);
}
