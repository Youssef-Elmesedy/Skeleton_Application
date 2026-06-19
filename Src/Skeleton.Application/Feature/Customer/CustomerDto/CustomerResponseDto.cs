namespace Skeleton.Application.Feature.Customer.CustomerDto;

public record CustomerResponseDto(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string? Email,
    string? Address,
    string? Notes,
    bool IsActive,
    DateTime? CreateDate,
    string CreateBy,
    DateTime? ModifiedDate,
    string ModifiedBy
);