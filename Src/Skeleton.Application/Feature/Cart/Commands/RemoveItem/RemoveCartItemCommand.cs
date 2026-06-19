using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Cart.Commands.RemoveItem;

public record RemoveCartItemCommand(Guid CustomerId, Guid ProductId) : IRequest<Result<bool>>;

public sealed class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, Result<bool>>
{
    private readonly ICartService _cartService;

    public RemoveCartItemCommandHandler(ICartService cartService) => _cartService = cartService;

    public async Task<Result<bool>> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _cartService.RemoveItemAsync(request.CustomerId, request.ProductId, cancellationToken);
            return Result<bool>.Success("Item removed from cart successfully.", true);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<bool>(ex);
        }
    }
}
