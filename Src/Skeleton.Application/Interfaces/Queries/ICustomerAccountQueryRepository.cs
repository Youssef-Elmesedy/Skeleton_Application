using Skeleton.Application.Feature.CustomerAccount.AccountDto;

namespace Skeleton.Application.Interfaces.Queries;

public interface ICustomerAccountQueryRepository
{
    Task<CustomerAccountResponseDto?> GetAccountByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AccountTransactionResponseDto>> GetTransactionsByAccountIdAsync(Guid accountId, CancellationToken cancellationToken);
}
