using MediatR;
using Skeleton.Application.Feature.Customer.CustomerDto;

namespace Skeleton.Application.Feature.Customer.Queries.GetByStatus;

public record GetCustomersByStatusQuery(bool IsActive) : IRequest<Result<IReadOnlyList<CustomerResponseDto>>>;
public sealed class GetCustomersByStatusQueryHandler : IRequestHandler<GetCustomersByStatusQuery, Result<IReadOnlyList<CustomerResponseDto>>>
{
    private readonly ICustomerService _customerService;
    public GetCustomersByStatusQueryHandler(ICustomerService customerService) => _customerService = customerService;

    public async Task<Result<IReadOnlyList<CustomerResponseDto>>> Handle(GetCustomersByStatusQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetCustomersByStatusAsync(request.IsActive, cancellationToken);
        return Result<IReadOnlyList<CustomerResponseDto>>.Success("Customers retrieved successfully.", customers);
    }
}