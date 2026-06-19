using Skeleton.Application.Common;
using Skeleton.Application.Feature.Category.CategoryDto;
using Skeleton.Application.Feature.Customer.CustomerDto;
using Skeleton.Application.Feature.Product.ProductDto;
using Skeleton.Domain.Entities;
using System.Linq.Expressions;

namespace Skeleton.Infrastructure.Common.ProjectionsExtensions;

public static class AsProjections
{
    public static Expression<Func<Product, ProductResponseDto>> AsProductResponseDto =>
        p => new ProductResponseDto
        (
            p.Id,
            p.FullName,
            p.Description,
            p.Price,
            p.Discount,
            p.StockQuantity,
            p.CategoryId ?? Guid.Empty,
            p.Category != null ? p.Category.Name : "No Category",
            p.RequiresPrescription,
            p.CreateDate,
            p.CreateBy.OrDefault("Not User"),
            p.ModifiedDate,
            p.ModifiedBy.OrDefault("Not Modifide")
        );

    public static Expression<Func<Product, ProductsLowStokResponseDto>> AsProdutslowStockDto =>
        p => new ProductsLowStokResponseDto
        (
            p.Id,
            p.FullName,
            p.Price,
            p.Discount,
            p.StockQuantity,
            p.Category != null ? p.Category.Name : "Not Assigen Category"

        );

    public static Expression<Func<Category, CategoryResponseDto>> AsCategoryResponseDto =>
        p => new CategoryResponseDto
        (
            p.Id,
            p.Name,
            p.Description,
            p.Products.Count,
            p.CreateDate,
            p.CreateBy.OrDefault("Not User"),
            p.ModifiedDate,
            p.ModifiedBy.OrDefault("Not Modifide")
        );

    public static Expression<Func<Customer, CustomerResponseDto>> AsCustomerResponseDto =>
        p => new CustomerResponseDto
        (
            p.Id,
            p.FullName,
            p.PhoneNumber,
            p.Email,
            p.Address,
            p.Notes,
            p.IsActive,
            p.CreateDate,
            p.CreateBy.OrDefault("Not User"),
            p.ModifiedDate,
            p.ModifiedBy.OrDefault("Not Modifide")
        );
}
