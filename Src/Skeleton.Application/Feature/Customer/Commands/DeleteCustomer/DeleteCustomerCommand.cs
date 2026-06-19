using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Customer.Commands.DeleteCustomer;

public record DeleteCustomerCommand(Guid CustomerId) : IRequest<Result<bool>>;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<bool>>
{
    private readonly ICustomerService _customerService;

    public DeleteCustomerCommandHandler(ICustomerService customerService) => _customerService = customerService;

    public async Task<Result<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _customerService.DeleteCustomerAsync(request.CustomerId, cancellationToken);
            return Result<bool>.Success("Customer deleted successfully.", true);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<bool>(ex);
        }
    }
}
