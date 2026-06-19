using Skeleton.Application.Feature.CustomerAccount.AccountDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.BusinessRules;
using Skeleton.Domain.Entities;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Application.Services.ImplementationServices;

/// <summary>
/// يتعامل مع حساب العميل المالي (رصيد + معاملات)
/// </summary>
public class CustomerAccountService : ICustomerAccountService
{
    private readonly IReadRepository<CustomerAccount> _accountReadRepository;
    private readonly IWriteRepository<CustomerAccount> _accountWriteRepository;
    private readonly ICustomerAccountQueryRepository _queryRepository;
    private readonly CustomerAccountBusinessRules _businessRules;

    public CustomerAccountService(
        IReadRepository<CustomerAccount> accountReadRepository,
        IWriteRepository<CustomerAccount> accountWriteRepository,
        ICustomerAccountQueryRepository queryRepository,
        CustomerAccountBusinessRules businessRules)
    {
        _accountReadRepository = accountReadRepository;
        _accountWriteRepository = accountWriteRepository;
        _queryRepository = queryRepository;
        _businessRules = businessRules;
    }

    /// <summary>
    /// عرض رصيد وتاريخ معاملات العميل
    /// </summary>
    public async Task<CustomerAccountResponseDto> GetAccountByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        await _businessRules.EnsureAccountExistsAsync(customerId, cancellationToken);

        var account = await _queryRepository.GetAccountByCustomerIdAsync(customerId, cancellationToken);
        return account!;
    }

    /// <summary>
    /// إضافة دين على العميل (Debit)
    /// </summary>
    public async Task<CustomerAccountResponseDto> AddDebitAsync(
        Guid customerId,
        AddDebitDto dto,
        CancellationToken cancellationToken)
    {
        await _businessRules.EnsureAccountExistsAsync(customerId, cancellationToken);

        var account = await _accountReadRepository.FirstOrDefaultAsync(
            a => a.CustomerId == customerId, cancellationToken);

        account!.AddDebit(dto.Amount, dto.Description);

        await _accountWriteRepository.UpdateAsync(account);
        await _accountWriteRepository.SaveChangesAsync(cancellationToken);

        return (await _queryRepository.GetAccountByCustomerIdAsync(customerId, cancellationToken))!;
    }

    /// <summary>
    /// تسجيل دفعة من العميل (Credit)
    /// </summary>
    public async Task<CustomerAccountResponseDto> AddPaymentAsync(
        Guid customerId,
        AddPaymentDto dto,
        CancellationToken cancellationToken)
    {
        await _businessRules.EnsureAccountExistsAsync(customerId, cancellationToken);
        await _businessRules.EnsureSufficientBalanceAsync(customerId, dto.Amount, cancellationToken);

        var account = await _accountReadRepository.FirstOrDefaultAsync(
            a => a.CustomerId == customerId, cancellationToken);

        account!.AddPayment(dto.Amount, dto.Description);

        await _accountWriteRepository.UpdateAsync(account);
        await _accountWriteRepository.SaveChangesAsync(cancellationToken);

        return (await _queryRepository.GetAccountByCustomerIdAsync(customerId, cancellationToken))!;
    }
}
