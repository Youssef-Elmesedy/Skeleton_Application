using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.Entities;

public class Cart : BaseEntity
{
    public Guid CustomerId { get; private set; }

    private readonly List<CartItem> _items = new();
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    public decimal TotalPrice => _items.Sum(i => i.TotalPrice);

    public Cart(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new BusinessException(ErrorType.Validation, "CustomerId is required.");

        CustomerId = customerId;
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
            existing.IncreaseQuantity(quantity);
        else
            _items.Add(new CartItem(Id, productId, productName, unitPrice, quantity));
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
            _items.Remove(item);
    }

    public void Clear() => _items.Clear();
}
