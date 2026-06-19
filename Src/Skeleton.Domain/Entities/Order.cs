using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public string OrderNumber { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? CouponCode { get; private set; }
    public string? Notes { get; private set; }
    public DateTime OrderDate { get; private set; }

    public Customer Customer { get; private set; } = null!;

    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public Payment? Payment { get; private set; }

    public Order(Guid customerId, string? notes = null, string? couponCode = null)
    {
        if (customerId == Guid.Empty)
            throw new BusinessException(ErrorType.Validation, "CustomerId is required.");

        CustomerId = customerId;
        OrderNumber = GenerateOrderNumber();
        Status = OrderStatus.Pending;
        Notes = notes;
        CouponCode = couponCode?.ToUpper().Trim();
        OrderDate = DateTime.UtcNow;
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        _items.Add(new OrderItem(Id, productId, productName, unitPrice, quantity));
        RecalculateTotals();
    }

    public void ApplyDiscount(decimal discountAmount)
    {
        if (discountAmount < 0)
            throw new BusinessException(ErrorType.Validation, "Discount amount cannot be negative.");

        DiscountAmount = discountAmount;
        RecalculateTotals();
    }

    public void UpdateStatus(OrderStatus newStatus) => Status = newStatus;

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new BusinessException(ErrorType.Validation, "Cannot cancel an order that has been shipped or delivered.");

        Status = OrderStatus.Cancelled;
    }

    private void RecalculateTotals()
    {
        SubTotal = _items.Sum(i => i.TotalPrice);
        TotalAmount = SubTotal - DiscountAmount;
        if (TotalAmount < 0) TotalAmount = 0;
    }

    private static string GenerateOrderNumber()
        => $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}
