using Skeleton.Application.Feature.Order.OrderDto;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(Guid customerId, CreateOrderDto dto, CancellationToken cancellationToken);
    Task<OrderResponseDto> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken);
    Task CancelOrderAsync(Guid orderId, CancellationToken cancellationToken);
    Task<OrderResponseDto> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrderResponseDto>> GetAllOrdersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<OrderResponseDto>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<PagedResult<OrderResponseDto>> GetPagedOrdersAsync(int page, int pageSize, CancellationToken cancellationToken);
}
