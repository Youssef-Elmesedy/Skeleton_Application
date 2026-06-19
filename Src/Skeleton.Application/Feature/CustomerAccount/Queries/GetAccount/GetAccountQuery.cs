using MediatR;
using Skeleton.Application.Behaviors;

using Skeleton.Application.Feature.CustomerAccount.AccountDto;

namespace Skeleton.Application.Feature.CustomerAccount.Queries.GetAccount;

// ── Query ────────────────────────────────────────────────────────
public record GetAccountByCustomerIdQuery(Guid CustomerId)
    : IRequest<Result<CustomerAccountResponseDto>>;

// ── Handler ──────────────────────────────────────────────────────
public sealed class GetAccountByCustomerIdQueryHandler
    : IRequestHandler<GetAccountByCustomerIdQuery, Result<CustomerAccountResponseDto>>
{
    private readonly ICustomerAccountService _service;

    public GetAccountByCustomerIdQueryHandler(ICustomerAccountService service)
        => _service = service;

    public async Task<Result<CustomerAccountResponseDto>> Handle(
        GetAccountByCustomerIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var account = await _service.GetAccountByCustomerIdAsync(
                request.CustomerId, cancellationToken);

            return Result<CustomerAccountResponseDto>
                .Success("Account retrieved successfully.", account);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CustomerAccountResponseDto>(ex);
        }
    }
}
