using Skeleton.Application.Feature.Cart.CartDto;

namespace Skeleton.Application.Services.Interfaces;

public interface ICartService
{
    Task<CartResponseDto> GetCartByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<CartResponseDto> AddItemAsync(Guid customerId, AddCartItemDto dto, CancellationToken cancellationToken);
    Task<CartResponseDto> UpdateItemQuantityAsync(Guid customerId, UpdateCartItemQuantityDto dto, CancellationToken cancellationToken);
    Task RemoveItemAsync(Guid customerId, Guid productId, CancellationToken cancellationToken);
    Task ClearCartAsync(Guid customerId, CancellationToken cancellationToken);
}
