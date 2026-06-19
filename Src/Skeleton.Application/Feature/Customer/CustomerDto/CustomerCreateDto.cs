namespace Skeleton.Application.Feature.Customer.CustomerDto;

public record CustomerCreateDto(
string FullName,
string PhoneNumber,
string? Email,
string? Address,
string? Notes
);
