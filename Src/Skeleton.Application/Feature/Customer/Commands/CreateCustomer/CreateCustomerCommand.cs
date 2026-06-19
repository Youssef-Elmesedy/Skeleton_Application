using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Customer.CustomerDto;

namespace Skeleton.Application.Feature.Customer.Commands.CreateCustomer;

public record CreateCustomerCommand(CustomerCreateDto Dto) : IRequest<Result<CustomerResponseDto>>;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerResponseDto>>
{
    private readonly ICustomerService _customerService;

    public CreateCustomerCommandHandler(ICustomerService customerService) => _customerService = customerService;

    public async Task<Result<CustomerResponseDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _customerService.AddCustomerAsync(request.Dto, cancellationToken);
            return Result<CustomerResponseDto>.Success("Customer created successfully.", customer);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CustomerResponseDto>(ex);
        }
    }
}

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Dto.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.PhoneNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Dto.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Dto.Email));
    }
}
