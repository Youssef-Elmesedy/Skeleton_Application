using Mapster;
using Skeleton.Application.Feature.CustomerAccount.AccountDto;

namespace Skeleton.Application.Feature.CustomerAccount.Mapping;

public static class CustomerAccountMapping
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Entities.CustomerAccount, CustomerAccountResponseDto>()
              .Map(dest => dest.CustomerName,
                   src => src.Customer != null ? src.Customer.FullName : string.Empty)
              .Map(dest => dest.Transactions, src => src.Transactions);

        config.NewConfig<AccountTransaction, AccountTransactionResponseDto>()
              .Map(dest => dest.Type, src => src.Type.ToString());
    }
}
