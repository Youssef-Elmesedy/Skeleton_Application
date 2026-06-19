namespace Skeleton.Application.Feature.Product.ProductDto;

public record ProductResponseDto(
    Guid ProductId,
    string? FullName,
    string? Description,
    decimal Price,
    decimal Discount,
    decimal StockQuantity,
    Guid? CategoryId,
    string? CategoryName,
    bool? RequiresPrescription,
    DateTime? CreateDate,
    string? CreateBy,
    DateTime? ModifiedDate,
    string? ModifiedBy
);
public record ProductsLowStokResponseDto(
    Guid ProductId,
    string? FullName,
    decimal Price,
    decimal Discount,
    decimal StockQuantity,
    string CategoryName
);


