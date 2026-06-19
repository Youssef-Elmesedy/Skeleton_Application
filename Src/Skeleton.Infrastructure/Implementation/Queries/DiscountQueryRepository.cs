using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Feature.Discount.DiscountDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.Implementation.Queries;

// ════════════════════════════════════════
//  Discount Query Repository
// ════════════════════════════════════════
internal class DiscountQueryRepository : IDiscountQueryRepository
{
    private readonly AppDbContext _context;
    public DiscountQueryRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<DiscountResponseDto>> GetAllDiscountsAsync(CancellationToken cancellationToken)
    {
        return await _context.Discounts
            .AsNoTracking()
            .OrderByDescending(d => d.CreateDate)
            .Select(d => new DiscountResponseDto(
                d.Id, d.Code, d.Description, d.Type, d.Value,
                d.MinOrderAmount, d.MaxDiscountAmount, d.UsageLimit,
                d.UsageCount, d.StartDate, d.EndDate, d.IsActive, d.CreateDate))
            .ToListAsync(cancellationToken);
    }

    public async Task<DiscountResponseDto?> GetDiscountByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Discounts
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DiscountResponseDto(
                d.Id, d.Code, d.Description, d.Type, d.Value,
                d.MinOrderAmount, d.MaxDiscountAmount, d.UsageLimit,
                d.UsageCount, d.StartDate, d.EndDate, d.IsActive, d.CreateDate))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DiscountResponseDto?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await _context.Discounts
            .AsNoTracking()
            .Where(d => d.Code == code.ToUpper().Trim())
            .Select(d => new DiscountResponseDto(
                d.Id, d.Code, d.Description, d.Type, d.Value,
                d.MinOrderAmount, d.MaxDiscountAmount, d.UsageLimit,
                d.UsageCount, d.StartDate, d.EndDate, d.IsActive, d.CreateDate))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
