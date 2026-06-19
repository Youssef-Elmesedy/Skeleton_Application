namespace Skeleton.Application.Feature.Customer.CustomerDto;

public record CustomerUpdateDto(
    Guid Id,
    string? FullName,
    string? PhoneNumber,
    string? Email,
    string? Address,
    string? Notes
);
