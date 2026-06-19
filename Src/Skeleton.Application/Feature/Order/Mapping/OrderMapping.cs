using Mapster;
using Skeleton.Application.Feature.Order.OrderDto;

namespace Skeleton.Application.Feature.Order.Mapping;

public static class OrderMapping
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Entities.OrderItem, OrderItemResponseDto>()
            .Map(dest => dest.TotalPrice, src => src.TotalPrice);

        config.NewConfig<Domain.Entities.Order, OrderResponseDto>()
            .Map(dest => dest.CustomerFullName, src => src.Customer != null ? src.Customer.FullName : string.Empty)
            .Map(dest => dest.Items, src => src.Items.Adapt<List<OrderItemResponseDto>>());
    }
}
