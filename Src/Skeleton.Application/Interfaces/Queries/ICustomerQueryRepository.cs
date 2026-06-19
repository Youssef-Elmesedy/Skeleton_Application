using Skeleton.Application.Feature.Customer.CustomerDto;

namespace Skeleton.Application.Interfaces.Queries;

public interface ICustomerQueryRepository
{
    Task<CustomerResponseDto?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<CustomerResponseDto>> GetAllCustomersAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<CustomerResponseDto>> SearchAsync(string keyword, CancellationToken cancellationToken);

    Task<PagedResult<CustomerResponseDto>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CustomerResponseDto>> GetCustomersByStatusAsync(
    bool isActive,
    CancellationToken cancellationToken);
}
