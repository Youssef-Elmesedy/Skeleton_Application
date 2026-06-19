using Mapster;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Mapping;

public static class CategoryMapping
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Entities.Category, CategoryResponseDto>();

    }
}
