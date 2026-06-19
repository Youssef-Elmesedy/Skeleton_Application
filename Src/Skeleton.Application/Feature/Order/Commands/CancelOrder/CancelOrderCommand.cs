using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Order.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId) : IRequest<Result<bool>>;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<bool>>
{
    private readonly IOrderService _orderService;

    public CancelOrderCommandHandler(IOrderService orderService) => _orderService = orderService;

    public async Task<Result<bool>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _orderService.CancelOrderAsync(request.OrderId, cancellationToken);
            return Result<bool>.Success("Order cancelled successfully.", true);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<bool>(ex);
        }
    }
}
