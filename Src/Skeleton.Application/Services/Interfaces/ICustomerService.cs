using Skeleton.Application.Feature.Customer.CustomerDto;

namespace Skeleton.Application.Services.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponseDto> AddCustomerAsync(
        CustomerCreateDto dto,
        CancellationToken cancellationToken);

    Task<CustomerResponseDto> UpdateCustomerAsync(
        CustomerUpdateDto dto,
        CancellationToken cancellationToken);

    Task DeleteCustomerAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<CustomerResponseDto> GetCustomerByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CustomerResponseDto>> GetAllCustomersAsync(
        CancellationToken cancellationToken);

    Task<PagedResult<CustomerResponseDto>> GetPagedCustomersAsync(
        int page,
        int size,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CustomerResponseDto>> SearchCustomersAsync(
        string keyword,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CustomerResponseDto>> GetCustomersByStatusAsync(
    bool isActive,
    CancellationToken cancellationToken);

}