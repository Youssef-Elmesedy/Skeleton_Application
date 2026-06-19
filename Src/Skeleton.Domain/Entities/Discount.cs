using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.Entities;

public class Discount : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public DiscountType Type { get; private set; }
    public decimal Value { get; private set; }
    public decimal? MinOrderAmount { get; private set; }
    public decimal? MaxDiscountAmount { get; private set; }
    public int? UsageLimit { get; private set; }
    public int UsageCount { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }

    public Discount(
        string code,
        string description,
        DiscountType type,
        decimal value,
        DateTime startDate,
        DateTime endDate,
        decimal? minOrderAmount = null,
        decimal? maxDiscountAmount = null,
        int? usageLimit = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new BusinessException(ErrorType.Validation, "Coupon code is required.");
        if (value <= 0)
            throw new BusinessException(ErrorType.Validation, "Discount value must be greater than zero.");
        if (type == DiscountType.Percentage && value > 100)
            throw new BusinessException(ErrorType.Validation, "Percentage discount cannot exceed 100%.");
        if (startDate >= endDate)
            throw new BusinessException(ErrorType.Validation, "Start date must be before end date.");

        Code = code.ToUpper().Trim();
        Description = description;
        Type = type;
        Value = value;
        StartDate = startDate;
        EndDate = endDate;
        MinOrderAmount = minOrderAmount;
        MaxDiscountAmount = maxDiscountAmount;
        UsageLimit = usageLimit;
        IsActive = true;
        UsageCount = 0;
    }

    public bool IsValid(decimal orderAmount)
    {
        if (!IsActive) return false;
        if (DateTime.UtcNow < StartDate || DateTime.UtcNow > EndDate) return false;
        if (UsageLimit.HasValue && UsageCount >= UsageLimit.Value) return false;
        if (MinOrderAmount.HasValue && orderAmount < MinOrderAmount.Value) return false;
        return true;
    }

    public decimal CalculateDiscount(decimal orderAmount)
    {
        var discount = Type == DiscountType.Percentage
            ? orderAmount * (Value / 100)
            : Value;

        if (MaxDiscountAmount.HasValue)
            discount = Math.Min(discount, MaxDiscountAmount.Value);

        return Math.Min(discount, orderAmount);
    }

    public void Update(string description, decimal value, DateTime endDate, bool isActive)
    {
        Description = description;
        Value = value;
        EndDate = endDate;
        IsActive = isActive;
    }

    public void IncrementUsage() => UsageCount++;
    public void Deactivate() => IsActive = false;
}
