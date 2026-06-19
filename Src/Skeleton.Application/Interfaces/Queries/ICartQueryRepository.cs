using Skeleton.Application.Feature.Cart.CartDto;

namespace Skeleton.Application.Interfaces.Queries;

public interface ICartQueryRepository
{
    Task<CartResponseDto?> GetCartByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<Cart?> GetCartWithItemsAsync(Guid customerId, CancellationToken cancellationToken);
}
