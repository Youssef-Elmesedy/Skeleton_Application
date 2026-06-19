using Skeleton.Application.Feature.Cart.CartDto;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.ImplementationServices;

public class CartService : ICartService
{
    private readonly IReadRepository<Cart> _readRepository;
    private readonly IWriteRepository<Cart> _writeRepository;
    private readonly IReadRepository<Product> _productReadRepository;
    private readonly ICartQueryRepository _queryRepository;
    private readonly IMapper _mapper;

    public CartService(
        IReadRepository<Cart> readRepository,
        IWriteRepository<Cart> writeRepository,
        IReadRepository<Product> productReadRepository,
        ICartQueryRepository queryRepository,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _productReadRepository = productReadRepository;
        _queryRepository = queryRepository;
        _mapper = mapper;
    }

    public async Task<CartResponseDto> GetCartByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var cart = await _queryRepository.GetCartByCustomerIdAsync(customerId, cancellationToken);
        if (cart is null)
            return new CartResponseDto(Guid.Empty, customerId, new List<CartItemResponseDto>(), 0);

        return cart;
    }

    public async Task<CartResponseDto> AddItemAsync(Guid customerId, AddCartItemDto dto, CancellationToken cancellationToken)
    {
        var product = await _productReadRepository.GetByIdAsync(dto.ProductId, cancellationToken);
        DtoBusinessRules.EnsureExists(product, dto.ProductId, nameof(Product));

        var cart = await _readRepository.FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart(customerId);
            cart.AddItem(product!.Id, product.FullName, product.Price, dto.Quantity);
            await _writeRepository.AddAsync(cart);
        }
        else
        {
            cart.AddItem(product!.Id, product.FullName, product.Price, dto.Quantity);
            await _writeRepository.UpdateAsync(cart);
        }

        await _writeRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<CartResponseDto>(cart);
    }

    public async Task<CartResponseDto> UpdateItemQuantityAsync(Guid customerId, UpdateCartItemQuantityDto dto, CancellationToken cancellationToken)
    {
        var cart = await _queryRepository.GetCartWithItemsAsync(customerId, cancellationToken);
        DtoBusinessRules.EnsureExists(cart, $"Cart for customer {customerId} was not found.");

        var item = cart!.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (item is null)
            throw new BusinessException(ErrorType.NotFound, "product not found in cart.");


        item.UpdateQuantity(dto.Quantity);
        await _writeRepository.UpdateAsync(cart);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CartResponseDto>(cart);
    }

    public async Task RemoveItemAsync(Guid customerId, Guid productId, CancellationToken cancellationToken)
    {
        var cart = await _readRepository.FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
        DtoBusinessRules.EnsureExists(cart, $"Cart for customer {customerId} was not found.");

        cart!.RemoveItem(productId);
        await _writeRepository.UpdateAsync(cart);
        await _writeRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearCartAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var cart = await _readRepository.FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
        DtoBusinessRules.EnsureExists(cart, $"Cart for customer {customerId} was not found.");

        cart!.Clear();
        await _writeRepository.UpdateAsync(cart);
        await _writeRepository.SaveChangesAsync(cancellationToken);
    }
}
