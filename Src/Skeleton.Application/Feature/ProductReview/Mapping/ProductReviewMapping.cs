using Mapster;
using Skeleton.Application.Feature.ProductReview.ReviewDto;

namespace Skeleton.Application.Feature.ProductReview.Mapping;

public static class ProductReviewMapping
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Entities.ProductReview, ReviewResponseDto>()
            .Map(dest => dest.CustomerFullName,
                 src => src.Customer != null ? src.Customer.FullName : string.Empty);
    }
}
