using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Feature.CustomerAccount.AccountDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.Implementation.Queries;

internal class CustomerAccountQueryRepository : ICustomerAccountQueryRepository
{
    private readonly AppDbContext _context;

    public CustomerAccountQueryRepository(AppDbContext context)
        => _context = context;

    /// <summary>
    /// Returns full account details with all transactions ordered by newest first.
    /// </summary>
    public async Task<CustomerAccountResponseDto?> GetAccountByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        return await _context.CustomerAccounts
            .AsNoTracking()
            .Include(a => a.Customer)
            .Include(a => a.Transactions)
            .Where(a => a.CustomerId == customerId)
            .Select(a => new CustomerAccountResponseDto(
                a.Id,
                a.CustomerId,
                a.Customer != null ? a.Customer.FullName : string.Empty,
                a.Balance,
                a.Transactions
                    .OrderByDescending(t => t.Date)
                    .Select(t => new AccountTransactionResponseDto(
                        t.Id,
                        t.Amount,
                        t.Type.ToString(),
                        t.Description,
                        t.Date))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Returns all transactions for a specific account ordered by newest first.
    /// </summary>
    public async Task<IReadOnlyList<AccountTransactionResponseDto>> GetTransactionsByAccountIdAsync(
        Guid accountId,
        CancellationToken cancellationToken)
    {
        return await _context.AccountTransactions
            .AsNoTracking()
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Date)
            .Select(t => new AccountTransactionResponseDto(
                t.Id,
                t.Amount,
                t.Type.ToString(),
                t.Description,
                t.Date))
            .ToListAsync(cancellationToken);
    }
}
