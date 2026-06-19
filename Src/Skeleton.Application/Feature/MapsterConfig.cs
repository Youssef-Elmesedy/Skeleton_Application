using Mapster;
using Skeleton.Application.Feature.Cart.Mapping;
using Skeleton.Application.Feature.Category.Mapping;
using Skeleton.Application.Feature.Customer.Mapping;
using Skeleton.Application.Feature.CustomerAccount.Mapping;
using Skeleton.Application.Feature.Discount.Mapping;
using Skeleton.Application.Feature.Order.Mapping;
using Skeleton.Application.Feature.Payment.Mapping;
using Skeleton.Application.Feature.Product.Mapping;
using Skeleton.Application.Feature.ProductReview.Mapping;

namespace Skeleton.Application.Feature;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        ProductMapping.Register(config);
        CategoryMapping.Register(config);
        CartMapping.Register(config);
        OrderMapping.Register(config);
        CustomerMapping.Register(config);
        CustomerAccountMapping.Register(config);
        DiscountMapping.Register(config);
        PaymentMapping.Register(config);
        ProductReviewMapping.Register(config);
    }
}
