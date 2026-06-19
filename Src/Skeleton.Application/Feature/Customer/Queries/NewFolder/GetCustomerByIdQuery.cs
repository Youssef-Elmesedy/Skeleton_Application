using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Customer.CustomerDto;

namespace Skeleton.Application.Feature.Customer.Queries.GetById;

public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<Result<CustomerResponseDto>>;
public sealed class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerResponseDto>>
{
    private readonly ICustomerService _customerService;
    public GetCustomerByIdQueryHandler(ICustomerService customerService) => _customerService = customerService;

    public async Task<Result<CustomerResponseDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerService.GetCustomerByIdAsync(request.CustomerId, cancellationToken);
            return Result<CustomerResponseDto>.Success("Customer retrieved successfully.", customer);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CustomerResponseDto>(ex);
        }
    }
}