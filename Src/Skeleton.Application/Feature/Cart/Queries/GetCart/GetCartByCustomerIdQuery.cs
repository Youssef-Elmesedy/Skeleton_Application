using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Cart.CartDto;

namespace Skeleton.Application.Feature.Cart.Queries.GetCart;

public record GetCartByCustomerIdQuery(Guid CustomerId) : IRequest<Result<CartResponseDto>>;

public sealed class GetCartByCustomerIdQueryHandler : IRequestHandler<GetCartByCustomerIdQuery, Result<CartResponseDto>>
{
    private readonly ICartService _cartService;

    public GetCartByCustomerIdQueryHandler(ICartService cartService) => _cartService = cartService;

    public async Task<Result<CartResponseDto>> Handle(GetCartByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cart = await _cartService.GetCartByCustomerIdAsync(request.CustomerId, cancellationToken);
            return Result<CartResponseDto>.Success("Cart retrieved successfully.", cart);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CartResponseDto>(ex);
        }
    }
}
