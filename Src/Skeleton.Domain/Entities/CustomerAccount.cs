using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.Entities;

public class CustomerAccount : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public decimal Balance { get; private set; } = 0;

    // Navigation Properties
    public Customer? Customer { get; private set; }

    private readonly List<AccountTransaction> _transactions = new();
    public IReadOnlyList<AccountTransaction> Transactions => _transactions.AsReadOnly();

    public CustomerAccount(Guid customerId)
    {
        CustomerId = customerId;
    }

    // ── إضافة دين (العميل اشترى بالدين) ─────────────────────────
    public void AddDebit(decimal amount, string description)
    {
        if (amount <= 0)
            throw new BusinessException(ErrorType.Validation, "Amount must be positive.");

        Balance += amount;

        _transactions.Add(new AccountTransaction(
            CustomerId, Id, amount, TransactionType.Debit, description));
    }

    // ── تسجيل دفعة (العميل سدد جزء من الدين) ────────────────────
    public void AddPayment(decimal amount, string description)
    {
        if (amount <= 0)
            throw new BusinessException(ErrorType.Validation, "Payment must be positive.");

        if (amount > Balance)
            throw new BusinessException(ErrorType.Validation,
                $"Payment amount ({amount}) exceeds current balance ({Balance}).");

        Balance -= amount;

        _transactions.Add(new AccountTransaction(
            CustomerId, Id, amount, TransactionType.Credit, description));
    }
}
