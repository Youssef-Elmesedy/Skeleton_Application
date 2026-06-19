using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Cart.CartDto;

namespace Skeleton.Application.Feature.Cart.Commands.UpdateItemQuantity;

public record UpdateCartItemQuantityCommand(Guid CustomerId, UpdateCartItemQuantityDto Dto) : IRequest<Result<CartResponseDto>>;

public sealed class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, Result<CartResponseDto>>
{
    private readonly ICartService _cartService;

    public UpdateCartItemQuantityCommandHandler(ICartService cartService) => _cartService = cartService;

    public async Task<Result<CartResponseDto>> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await _cartService.UpdateItemQuantityAsync(request.CustomerId, request.Dto, cancellationToken);
            return Result<CartResponseDto>.Success("Cart updated successfully.", cart);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CartResponseDto>(ex);
        }
    }
}

public class UpdateCartItemQuantityValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Dto.ProductId).NotEmpty();
        RuleFor(x => x.Dto.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}
