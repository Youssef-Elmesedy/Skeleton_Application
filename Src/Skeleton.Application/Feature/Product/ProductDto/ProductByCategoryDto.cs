namespace Skeleton.Application.Feature.Product.ProductDto;

public record ProductByCategoryDto(Guid CategoryId, string CategoryName, List<ProductResponseDto> Products);
