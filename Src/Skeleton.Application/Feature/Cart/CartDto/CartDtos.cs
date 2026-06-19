namespace Skeleton.Application.Feature.Cart.CartDto;

public record CartItemResponseDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
);

public record CartResponseDto(
    Guid Id,
    Guid CustomerId,
    IReadOnlyList<CartItemResponseDto> Items,
    decimal TotalPrice
);

public record AddCartItemDto(
    Guid ProductId,
    int Quantity
);

public record UpdateCartItemQuantityDto(
    Guid ProductId,
    int Quantity
);
