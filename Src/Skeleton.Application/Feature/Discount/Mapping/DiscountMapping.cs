using Mapster;
using Skeleton.Application.Feature.Discount.DiscountDto;

namespace Skeleton.Application.Feature.Discount.Mapping;

public static class DiscountMapping
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Entities.Discount, DiscountResponseDto>();
    }
}
