using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    public Cart Cart { get; private set; } = null!;

    public CartItem(Guid cartId, Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (quantity <= 0)
            throw new BusinessException(ErrorType.Validation, "Quantity must be greater than zero.");
        if (unitPrice <= 0)
            throw new BusinessException(ErrorType.Validation, "Unit price must be greater than zero.");

        CartId = cartId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessException(ErrorType.Validation, "Quantity must be greater than zero.");
        Quantity += quantity;
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new BusinessException(ErrorType.Validation, "Quantity must be greater than zero.");
        Quantity = quantity;
    }
}
