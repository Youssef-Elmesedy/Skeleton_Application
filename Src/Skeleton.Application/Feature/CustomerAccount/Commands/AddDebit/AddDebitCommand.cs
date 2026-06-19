using MediatR;
using Skeleton.Application.Behaviors;

using Skeleton.Application.Feature.CustomerAccount.AccountDto;

namespace Skeleton.Application.Feature.CustomerAccount.Commands.AddDebit;

// ── Command ──────────────────────────────────────────────────────
public record AddDebitCommand(Guid CustomerId, AddDebitDto Dto)
    : IRequest<Result<CustomerAccountResponseDto>>;

// ── Handler ──────────────────────────────────────────────────────
public sealed class AddDebitCommandHandler
    : IRequestHandler<AddDebitCommand, Result<CustomerAccountResponseDto>>
{
    private readonly ICustomerAccountService _service;

    public AddDebitCommandHandler(ICustomerAccountService service)
        => _service = service;

    public async Task<Result<CustomerAccountResponseDto>> Handle(
        AddDebitCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var account = await _service.AddDebitAsync(
                request.CustomerId, request.Dto, cancellationToken);

            return Result<CustomerAccountResponseDto>
                .Success("Debit added successfully.", account);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CustomerAccountResponseDto>(ex);
        }
    }
}

// ── Validator ─────────────────────────────────────────────────────
public class AddDebitValidator : AbstractValidator<AddDebitCommand>
{
    public AddDebitValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");

        RuleFor(x => x.Dto.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Dto.Description)
            .NotEmpty().MaximumLength(500);
    }
}
