using MediatR;
using Skeleton.Application.Behaviors;

using Skeleton.Application.Feature.CustomerAccount.AccountDto;

namespace Skeleton.Application.Feature.CustomerAccount.Commands.AddPayment;

// ── Command ──────────────────────────────────────────────────────
public record AddPaymentCommand(Guid CustomerId, AddPaymentDto Dto)
    : IRequest<Result<CustomerAccountResponseDto>>;

// ── Handler ──────────────────────────────────────────────────────
public sealed class AddPaymentCommandHandler
    : IRequestHandler<AddPaymentCommand, Result<CustomerAccountResponseDto>>
{
    private readonly ICustomerAccountService _service;

    public AddPaymentCommandHandler(ICustomerAccountService service)
        => _service = service;

    public async Task<Result<CustomerAccountResponseDto>> Handle(
        AddPaymentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var account = await _service.AddPaymentAsync(
                request.CustomerId, request.Dto, cancellationToken);

            return Result<CustomerAccountResponseDto>
                .Success("Payment recorded successfully.", account);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CustomerAccountResponseDto>(ex);
        }
    }
}

// ── Validator ─────────────────────────────────────────────────────
public class AddPaymentValidator : AbstractValidator<AddPaymentCommand>
{
    public AddPaymentValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.Dto.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than zero.");

        RuleFor(x => x.Dto.Description)
            .NotEmpty().MaximumLength(500);
    }
}
