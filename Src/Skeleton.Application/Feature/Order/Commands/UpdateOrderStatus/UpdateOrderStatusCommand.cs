using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Order.OrderDto;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Feature.Order.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand(Guid OrderId, UpdateOrderStatusDto Dto) : IRequest<Result<OrderResponseDto>>;

public sealed class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result<OrderResponseDto>>
{
    private readonly IOrderService _orderService;

    public UpdateOrderStatusCommandHandler(IOrderService orderService) => _orderService = orderService;

    public async Task<Result<OrderResponseDto>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderService.UpdateOrderStatusAsync(request.OrderId, request.Dto.NewStatus, cancellationToken);
            return Result<OrderResponseDto>.Success("Order status updated successfully.", order);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<OrderResponseDto>(ex);
        }
    }
}
