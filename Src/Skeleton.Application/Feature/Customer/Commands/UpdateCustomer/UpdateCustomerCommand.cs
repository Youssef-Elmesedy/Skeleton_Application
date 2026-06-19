using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Customer.CustomerDto;

namespace Skeleton.Application.Feature.Customer.Commands.UpdateCustomer;

public record UpdateCustomerCommand(CustomerUpdateDto Dto) : IRequest<Result<CustomerResponseDto>>;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<CustomerResponseDto>>
{
    private readonly ICustomerService _customerService;

    public UpdateCustomerCommandHandler(ICustomerService customerService) => _customerService = customerService;

    public async Task<Result<CustomerResponseDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerService.UpdateCustomerAsync(request.Dto, cancellationToken);
            return Result<CustomerResponseDto>.Success("Customer updated successfully.", customer);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CustomerResponseDto>(ex);
        }
    }
}

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Dto.Id).NotEmpty();
        RuleFor(x => x.Dto.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Dto.Email));
    }
}
