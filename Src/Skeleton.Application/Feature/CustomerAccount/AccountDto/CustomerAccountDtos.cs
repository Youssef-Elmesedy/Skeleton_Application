namespace Skeleton.Application.Feature.CustomerAccount.AccountDto;

// ─── Response DTOs ──────────────────────────────────────────
public record CustomerAccountResponseDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    decimal Balance,
    IReadOnlyList<AccountTransactionResponseDto> Transactions
);

public record AccountTransactionResponseDto(
    Guid Id,
    decimal Amount,
    string Type,
    string Description,
    DateTime Date
);

// ─── Command DTOs ────────────────────────────────────────────
public record AddDebitDto(
    decimal Amount,
    string Description
);

public record AddPaymentDto(
    decimal Amount,
    string Description
);
