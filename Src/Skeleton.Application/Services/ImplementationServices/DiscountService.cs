using Skeleton.Application.Feature.Discount.DiscountDto;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.ImplementationServices;

// ════════════════════════════════════════
//  Discount Service
// ════════════════════════════════════════
public class DiscountService : IDiscountService
{
    private readonly IReadRepository<Domain.Entities.Discount> _readRepository;
    private readonly IWriteRepository<Domain.Entities.Discount> _writeRepository;
    private readonly IDiscountQueryRepository _queryRepository;
    private readonly IMapper _mapper;

    public DiscountService(
        IReadRepository<Domain.Entities.Discount> readRepository,
        IWriteRepository<Domain.Entities.Discount> writeRepository,
        IDiscountQueryRepository queryRepository,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _queryRepository = queryRepository;
        _mapper = mapper;
    }

    public async Task<DiscountResponseDto> CreateDiscountAsync(CreateDiscountDto dto, CancellationToken cancellationToken)
    {
        var exists = await _readRepository.AnyAsync(d => d.Code == dto.Code.ToUpper().Trim(), cancellationToken);
        if (exists)
            throw new BusinessException(ErrorType.Conflict, "A discount with this coupon code already exists.");

        var discount = new Domain.Entities.Discount(dto.Code, dto.Description, dto.Type, dto.Value,
            dto.StartDate, dto.EndDate, dto.MinOrderAmount, dto.MaxDiscountAmount, dto.UsageLimit);

        await _writeRepository.AddAsync(discount);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DiscountResponseDto>(discount);
    }

    public async Task<DiscountResponseDto> UpdateDiscountAsync(Guid id, UpdateDiscountDto dto, CancellationToken cancellationToken)
    {
        var discount = await _readRepository.GetByIdAsync(id, cancellationToken);
        DtoBusinessRules.EnsureExists(discount, id, nameof(Domain.Entities.Discount));

        discount!.Update(dto.Description, dto.Value, dto.EndDate, dto.IsActive);
        await _writeRepository.UpdateAsync(discount);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DiscountResponseDto>(discount);
    }

    public async Task DeleteDiscountAsync(Guid id, CancellationToken cancellationToken)
    {
        var discount = await _readRepository.GetByIdAsync(id, cancellationToken);
        DtoBusinessRules.EnsureExists(discount, id, nameof(Domain.Entities.Discount));

        await _writeRepository.DeleteAsync(discount!);
        await _writeRepository.SaveChangesAsync(cancellationToken);
    }

    public Task<IReadOnlyList<DiscountResponseDto>> GetAllDiscountsAsync(CancellationToken cancellationToken)
        => _queryRepository.GetAllDiscountsAsync(cancellationToken);

    public async Task<DiscountResponseDto> GetDiscountByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var discount = await _queryRepository.GetDiscountByIdAsync(id, cancellationToken);
        DtoBusinessRules.EnsureExists(discount, id, nameof(Domain.Entities.Discount));
        return discount!;
    }

    public async Task<ValidateCouponResponseDto> ValidateCouponAsync(string code, decimal orderAmount, CancellationToken cancellationToken)
    {
        var discount = await _readRepository.FirstOrDefaultAsync(d => d.Code == code.ToUpper().Trim(), cancellationToken);
        if (discount is null || !discount.IsValid(orderAmount))
            return new ValidateCouponResponseDto(false, "Coupon is invalid, expired, or minimum order amount not met.", 0);

        var amount = discount.CalculateDiscount(orderAmount);
        return new ValidateCouponResponseDto(true, null, amount);
    }
}
