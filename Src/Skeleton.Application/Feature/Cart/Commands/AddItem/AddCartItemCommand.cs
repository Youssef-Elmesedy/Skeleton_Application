using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Cart.CartDto;

namespace Skeleton.Application.Feature.Cart.Commands.AddItem;

public record AddCartItemCommand(Guid CustomerId, AddCartItemDto Dto) : IRequest<Result<CartResponseDto>>;

public sealed class AddCartItemCommandHandler : IRequestHandler<AddCartItemCommand, Result<CartResponseDto>>
{
    private readonly ICartService _cartService;

    public AddCartItemCommandHandler(ICartService cartService) => _cartService = cartService;

    public async Task<Result<CartResponseDto>> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await _cartService.AddItemAsync(request.CustomerId, request.Dto, cancellationToken);
            return Result<CartResponseDto>.Success("Item added to cart successfully.", cart);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CartResponseDto>(ex);
        }
    }
}

public class AddCartItemValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Dto.ProductId).NotEmpty();
        RuleFor(x => x.Dto.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}
