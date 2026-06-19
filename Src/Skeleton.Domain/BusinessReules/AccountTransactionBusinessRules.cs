using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Domain.BusinessRules;

public class AccountTransactionBusinessRules : BaseFinancialRules<AccountTransaction>
{
    private readonly IReadRepository<AccountTransaction> _transactionRepository;

    public AccountTransactionBusinessRules(IReadRepository<AccountTransaction> transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    // ====== تحقق من وجود المعاملة ======
    public async Task EnsureTransactionExistsAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        var exists = await _transactionRepository.AnyAsync(t => t.Id == transactionId, cancellationToken);
        if (!exists)
            throw new BusinessException(ErrorType.NotFound, "Transaction not found", nameof(AccountTransaction), transactionId);
    }

}
