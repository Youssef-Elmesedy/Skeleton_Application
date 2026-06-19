using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Order.OrderDto;

namespace Skeleton.Application.Feature.Order.Queries.GetAll;

public record GetAllOrdersQuery : IRequest<Result<IReadOnlyList<OrderResponseDto>>>;

public sealed class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<IReadOnlyList<OrderResponseDto>>>
{
    private readonly IOrderService _orderService;
    public GetAllOrdersQueryHandler(IOrderService orderService) => _orderService = orderService;

    public async Task<Result<IReadOnlyList<OrderResponseDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
        return Result<IReadOnlyList<OrderResponseDto>>.Success("Orders retrieved successfully.", orders);
    }
}

// ──────────────────────────────────────────────────────────────────

public record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<OrderResponseDto>>;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderResponseDto>>
{
    private readonly IOrderService _orderService;
    public GetOrderByIdQueryHandler(IOrderService orderService) => _orderService = orderService;

    public async Task<Result<OrderResponseDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(request.OrderId, cancellationToken);
            return Result<OrderResponseDto>.Success("Order retrieved successfully.", order);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<OrderResponseDto>(ex);
        }
    }
}

// ──────────────────────────────────────────────────────────────────

public record GetOrdersByCustomerQuery(Guid CustomerId) : IRequest<Result<IReadOnlyList<OrderResponseDto>>>;

public sealed class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, Result<IReadOnlyList<OrderResponseDto>>>
{
    private readonly IOrderService _orderService;
    public GetOrdersByCustomerQueryHandler(IOrderService orderService) => _orderService = orderService;

    public async Task<Result<IReadOnlyList<OrderResponseDto>>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetOrdersByCustomerIdAsync(request.CustomerId, cancellationToken);
        return Result<IReadOnlyList<OrderResponseDto>>.Success("Orders retrieved successfully.", orders);
    }
}

// ──────────────────────────────────────────────────────────────────

public record GetPagedOrdersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<OrderResponseDto>>>;

public sealed class GetPagedOrdersQueryHandler : IRequestHandler<GetPagedOrdersQuery, Result<PagedResult<OrderResponseDto>>>
{
    private readonly IOrderService _orderService;
    public GetPagedOrdersQueryHandler(IOrderService orderService) => _orderService = orderService;

    public async Task<Result<PagedResult<OrderResponseDto>>> Handle(GetPagedOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetPagedOrdersAsync(request.PageNumber, request.PageSize, cancellationToken);
        return Result<PagedResult<OrderResponseDto>>.Success("Orders retrieved successfully.", orders);
    }
}
