using Skeleton.Domain.BusinessRules;
using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Domain.BusinessReules;

public class OrderBusinessRules : EntityBaseBusinessRules<Order>
{
    private readonly IReadRepository<Cart> _cartRepository;

    public OrderBusinessRules(
        IReadRepository<Order> orderRepository,
        IReadRepository<Cart> cartRepository)
        : base(orderRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task EnsureCartHasItems(Guid customerId, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
        if (cart is null || !cart.Items.Any())
            throw new BusinessException(ErrorType.Validation, "Cannot create an order from an empty cart.");
    }
}
