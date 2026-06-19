using Skeleton.Domain.BusinessRules;
using Skeleton.Domain.Entities;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Domain.BusinessReules;

public class CartBusinessRules : EntityBaseBusinessRules<Cart>
{
    public CartBusinessRules(IReadRepository<Cart> cartRepository)
        : base(cartRepository)
    {
    }
}
