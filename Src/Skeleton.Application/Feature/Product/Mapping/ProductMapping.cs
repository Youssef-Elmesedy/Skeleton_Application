using Mapster;
namespace Skeleton.Application.Feature.Product.Mapping;

public static class ProductMapping
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Entities.Product, ProductResponseDto>()
              .Map(dest => dest.CategoryName,
              src => src.Category != null ? src.Category.Name : "No Category");

    }
}
