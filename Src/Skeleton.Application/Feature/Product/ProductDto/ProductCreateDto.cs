namespace Skeleton.Application.Feature.Product.ProductDto;
public class ProductCreateDto
{
    public string FullName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Discount { get; set; }
    public decimal StockQuantity { get; set; }
    public Guid? CategoryId { get; set; }
    public bool RequiresPrescription { get; set; }
}
