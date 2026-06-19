using Skeleton.Application.Feature.Order.OrderDto;

namespace Skeleton.Application.Interfaces.Queries;

public interface IOrderQueryRepository
{
    Task<OrderResponseDto?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrderResponseDto>> GetAllOrdersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<OrderResponseDto>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<PagedResult<OrderResponseDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
}
