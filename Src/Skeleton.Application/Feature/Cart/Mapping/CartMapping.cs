using Mapster;
using Skeleton.Application.Feature.Cart.CartDto;

namespace Skeleton.Application.Feature.Cart.Mapping;

public static class CartMapping
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Entities.CartItem, CartItemResponseDto>()
            .Map(dest => dest.TotalPrice, src => src.TotalPrice);

        config.NewConfig<Domain.Entities.Cart, CartResponseDto>()
            .Map(dest => dest.TotalPrice, src => src.TotalPrice)
            .Map(dest => dest.Items, src => src.Items.Adapt<List<CartItemResponseDto>>());

    }
}
