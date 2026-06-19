using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Feature.Cart.CartDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Domain.Entities;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.Implementation.Queries;

// ════════════════════════════════════════
//  Cart Query Repository
// ════════════════════════════════════════
internal class CartQueryRepository : ICartQueryRepository
{
    private readonly AppDbContext _context;
    public CartQueryRepository(AppDbContext context) => _context = context;

    public async Task<CartResponseDto?> GetCartByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .AsNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);

        if (cart is null) return null;

        return new CartResponseDto(
            cart.Id,
            cart.CustomerId,
            cart.Items.Select(i => new CartItemResponseDto(
                i.Id, i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.TotalPrice
            )).ToList(),
            cart.TotalPrice
        );
    }
    public async Task<Cart?> GetCartWithItemsAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
    }
}
