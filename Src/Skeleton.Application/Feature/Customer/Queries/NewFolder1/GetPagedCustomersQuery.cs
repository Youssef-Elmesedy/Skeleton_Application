using MediatR;
using Skeleton.Application.Feature.Customer.CustomerDto;

namespace Skeleton.Application.Feature.Customer.Queries.GetPaged;

public record GetPagedCustomersQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<CustomerResponseDto>>>;

public sealed class GetPagedCustomersQueryHandler : IRequestHandler<GetPagedCustomersQuery, Result<PagedResult<CustomerResponseDto>>>
{
    private readonly ICustomerService _customerService;
    public GetPagedCustomersQueryHandler(ICustomerService customerService) => _customerService = customerService;

    public async Task<Result<PagedResult<CustomerResponseDto>>> Handle(GetPagedCustomersQuery request, CancellationToken cancellationToken)
    {
        var result = await _customerService.GetPagedCustomersAsync(request.PageNumber, request.PageSize, cancellationToken);
        return Result<PagedResult<CustomerResponseDto>>.Success("Customers retrieved successfully.", result);
    }
}