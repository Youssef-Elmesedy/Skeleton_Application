using MediatR;
using Skeleton.Application.Feature.Customer.CustomerDto;

namespace Skeleton.Application.Feature.Customer.Queries.GetAll;

public record GetAllCustomersQuery : IRequest<Result<IReadOnlyList<CustomerResponseDto>>>;

public sealed class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, Result<IReadOnlyList<CustomerResponseDto>>>
{
    private readonly ICustomerService _customerService;
    public GetAllCustomersQueryHandler(ICustomerService customerService) => _customerService = customerService;

    public async Task<Result<IReadOnlyList<CustomerResponseDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAllCustomersAsync(cancellationToken);
        return Result<IReadOnlyList<CustomerResponseDto>>.Success("Customers retrieved successfully.", customers);
    }
}

