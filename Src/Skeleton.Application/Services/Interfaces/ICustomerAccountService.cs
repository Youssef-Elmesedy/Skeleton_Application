using Skeleton.Application.Feature.CustomerAccount.AccountDto;

namespace Skeleton.Application.Services.Interfaces;

public interface ICustomerAccountService
{
    Task<CustomerAccountResponseDto> GetAccountByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<CustomerAccountResponseDto> AddDebitAsync(Guid customerId, AddDebitDto dto, CancellationToken cancellationToken);
    Task<CustomerAccountResponseDto> AddPaymentAsync(Guid customerId, AddPaymentDto dto, CancellationToken cancellationToken);
}
