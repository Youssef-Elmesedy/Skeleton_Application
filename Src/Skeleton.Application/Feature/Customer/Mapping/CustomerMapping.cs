using Mapster;
using Skeleton.Application.Feature.Customer.CustomerDto;

namespace Skeleton.Application.Feature.Customer.Mapping;

public static class CustomerMapping
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Entities.Customer, CustomerResponseDto>();
    }
}
