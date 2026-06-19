using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Cart.Commands.ClearCart;

public record ClearCartCommand(Guid CustomerId) : IRequest<Result<bool>>;

public sealed class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Result<bool>>
{
    private readonly ICartService _cartService;

    public ClearCartCommandHandler(ICartService cartService) => _cartService = cartService;

    public async Task<Result<bool>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _cartService.ClearCartAsync(request.CustomerId, cancellationToken);
            return Result<bool>.Success("Cart cleared successfully.", true);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<bool>(ex);
        }
    }
}
