using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Domain.BusinessRules;

public class CustomerAccountBusinessRules : BaseFinancialRules<CustomerAccount>
{
    private readonly IReadRepository<CustomerAccount> _accountRepository;

    public CustomerAccountBusinessRules(IReadRepository<CustomerAccount> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    // ====== تحقق من وجود الحساب ======
    public async Task EnsureAccountExistsAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var exists = await _accountRepository.AnyAsync(a => a.CustomerId == customerId, cancellationToken);
        if (!exists)
            throw new BusinessException(ErrorType.NotFound, "Customer account not found", nameof(CustomerAccount), customerId);
    }

    // ====== تحقق من الرصيد الكافي ======
    public async Task EnsureSufficientBalanceAsync(Guid customerId, decimal amount, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.FirstOrDefaultAsync(a => a.CustomerId == customerId, cancellationToken);
        if (account == null)
            throw new BusinessException(ErrorType.NotFound, "Customer account not found", nameof(CustomerAccount), customerId);

        if (account.Balance < amount)
            throw new BusinessException(ErrorType.Validation, "Insufficient balance");
    }

}
