using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Order.OrderDto;

namespace Skeleton.Application.Feature.Order.Commands.CreateOrder;

public record CreateOrderCommand(Guid CustomerId, CreateOrderDto Dto) : IRequest<Result<OrderResponseDto>>;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponseDto>>
{
    private readonly IOrderService _orderService;

    public CreateOrderCommandHandler(IOrderService orderService) => _orderService = orderService;

    public async Task<Result<OrderResponseDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(request.CustomerId, request.Dto, cancellationToken);
            return Result<OrderResponseDto>.Success("Order created successfully.", order);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<OrderResponseDto>(ex);
        }
    }
}

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Dto.CouponCode).MaximumLength(50).When(x => x.Dto.CouponCode is not null);
    }
}
