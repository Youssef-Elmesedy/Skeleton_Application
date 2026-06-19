using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Feature.Discount.DiscountDto;

public record DiscountResponseDto(
    Guid Id,
    string Code,
    string Description,
    DiscountType Type,
    decimal Value,
    decimal? MinOrderAmount,
    decimal? MaxDiscountAmount,
    int? UsageLimit,
    int UsageCount,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    DateTime? CreateDate
);

public record CreateDiscountDto(
    string Code,
    string Description,
    DiscountType Type,
    decimal Value,
    DateTime StartDate,
    DateTime EndDate,
    decimal? MinOrderAmount,
    decimal? MaxDiscountAmount,
    int? UsageLimit
);

public record UpdateDiscountDto(
    string Description,
    decimal Value,
    DateTime EndDate,
    bool IsActive
);

public record ValidateCouponResponseDto(
    bool IsValid,
    string? Message,
    decimal DiscountAmount
);
