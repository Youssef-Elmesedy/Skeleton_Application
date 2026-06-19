namespace Skeleton.Application.Feature.Product.ProductDto;

public class ProductUpdateDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public decimal? Discount { get; set; }
    public decimal? StockQuantity { get; set; }
    public Guid? CategoryId { get; set; } 
    public bool RequiresPrescription { get; set; }
}
