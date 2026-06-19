using Skeleton.Application.Feature.Order.OrderDto;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.ImplementationServices;

public class OrderService : IOrderService
{
    private readonly IReadRepository<Order> _readRepository;
    private readonly IWriteRepository<Order> _writeRepository;
    private readonly IReadRepository<Cart> _cartReadRepository;
    private readonly ICartQueryRepository _cartQueryReadRepository;
    private readonly IWriteRepository<Cart> _cartWriteRepository;
    private readonly IReadRepository<Discount> _discountReadRepository;
    private readonly IWriteRepository<Discount> _discountWriteRepository;
    private readonly IOrderQueryRepository _queryRepository;
    private readonly IMapper _mapper;

    public OrderService(
        IReadRepository<Order> readRepository,
        IWriteRepository<Order> writeRepository,
        IReadRepository<Cart> cartReadRepository,
        IWriteRepository<Cart> cartWriteRepository,
        IReadRepository<Discount> discountReadRepository,
        IWriteRepository<Discount> discountWriteRepository,
        IOrderQueryRepository queryRepository,
        IMapper mapper,
        ICartQueryRepository cartQueryReadRepository)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _cartReadRepository = cartReadRepository;
        _cartWriteRepository = cartWriteRepository;
        _discountReadRepository = discountReadRepository;
        _discountWriteRepository = discountWriteRepository;
        _queryRepository = queryRepository;
        _mapper = mapper;
        _cartQueryReadRepository = cartQueryReadRepository;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(Guid customerId, CreateOrderDto dto, CancellationToken cancellationToken)
    {
        var cart = await _cartQueryReadRepository.GetCartWithItemsAsync(customerId, cancellationToken);
        if (cart is null || !cart.Items.Any())
            throw new BusinessException(ErrorType.Validation, "Cannot create an order from an empty cart.");

        var order = new Order(customerId, dto.Notes, dto.CouponCode);

        foreach (var item in cart.Items)
            order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);

        // Apply discount if coupon code provided
        if (!string.IsNullOrWhiteSpace(dto.CouponCode))
        {
            var discount = await _discountReadRepository.FirstOrDefaultAsync(
                d => d.Code == dto.CouponCode.ToUpper().Trim(), cancellationToken);

            if (discount is not null && discount.IsValid(order.SubTotal))
            {
                order.ApplyDiscount(discount.CalculateDiscount(order.SubTotal));
                discount.IncrementUsage();
                await _discountWriteRepository.UpdateAsync(discount);
            }
        }

        await _writeRepository.AddAsync(order);

        // Clear cart after creating order
        cart.Clear();
        await _cartWriteRepository.UpdateAsync(cart);

        await _writeRepository.SaveChangesAsync(cancellationToken);

        var result = await _queryRepository.GetOrderByIdAsync(order.Id, cancellationToken);
        return result!;
    }

    public async Task<OrderResponseDto> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken cancellationToken)
    {
        var order = await _readRepository.GetByIdAsync(orderId, cancellationToken);
        DtoBusinessRules.EnsureExists(order, orderId, nameof(Order));

        order!.UpdateStatus(newStatus);
        await _writeRepository.UpdateAsync(order);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        var result = await _queryRepository.GetOrderByIdAsync(orderId, cancellationToken);
        return result!;
    }

    public async Task CancelOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _readRepository.GetByIdAsync(orderId, cancellationToken);
        DtoBusinessRules.EnsureExists(order, orderId, nameof(Order));

        order!.Cancel();
        await _writeRepository.UpdateAsync(order);
        await _writeRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrderResponseDto> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _queryRepository.GetOrderByIdAsync(id, cancellationToken);
        DtoBusinessRules.EnsureExists(order, id, nameof(Order));
        return order!;
    }

    public Task<IReadOnlyList<OrderResponseDto>> GetAllOrdersAsync(CancellationToken cancellationToken)
        => _queryRepository.GetAllOrdersAsync(cancellationToken);

    public Task<IReadOnlyList<OrderResponseDto>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
        => _queryRepository.GetOrdersByCustomerIdAsync(customerId, cancellationToken);

    public Task<PagedResult<OrderResponseDto>> GetPagedOrdersAsync(int page, int pageSize, CancellationToken cancellationToken)
        => _queryRepository.GetPagedAsync(page, pageSize, cancellationToken);
}
