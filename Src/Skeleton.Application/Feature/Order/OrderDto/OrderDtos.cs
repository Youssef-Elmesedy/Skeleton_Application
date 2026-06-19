using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Feature.Order.OrderDto;

public record OrderItemResponseDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
);

public record OrderResponseDto(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string CustomerFullName,
    OrderStatus Status,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal TotalAmount,
    string? CouponCode,
    string? Notes,
    DateTime OrderDate,
    IReadOnlyList<OrderItemResponseDto> Items,
    DateTime? CreateDate
);

public record CreateOrderDto(
    string? Notes,
    string? CouponCode
);

public record UpdateOrderStatusDto(
    OrderStatus NewStatus
);
