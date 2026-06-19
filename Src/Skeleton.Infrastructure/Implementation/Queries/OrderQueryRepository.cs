using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Common;
using Skeleton.Application.Feature.Order.OrderDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Infrastructure.Common.Extensions;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.Implementation.Queries;

// ════════════════════════════════════════
//  Order Query Repository
// ════════════════════════════════════════
internal class OrderQueryRepository : IOrderQueryRepository
{
    private readonly AppDbContext _context;
    public OrderQueryRepository(AppDbContext context) => _context = context;

    public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Where(o => o.Id == id)
            .Select(AsOrderResponseDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderResponseDto>> GetAllOrdersAsync(CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate)
            .Select(AsOrderResponseDto())
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderResponseDto>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .Select(AsOrderResponseDto())
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<OrderResponseDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate);

        return await query.ToPagedResultAsync(page, pageSize, AsOrderResponseDto());
    }

    private static System.Linq.Expressions.Expression<Func<Domain.Entities.Order, OrderResponseDto>> AsOrderResponseDto()
        => o => new OrderResponseDto(
            o.Id,
            o.OrderNumber,
            o.CustomerId,
            o.Customer != null ? o.Customer.FullName : string.Empty,
            o.Status,
            o.SubTotal,
            o.DiscountAmount,
            o.TotalAmount,
            o.CouponCode,
            o.Notes,
            o.OrderDate,
            o.Items.Select(i => new OrderItemResponseDto(
                i.Id, i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.TotalPrice
            )).ToList(),
            o.CreateDate
        );
}
