using Mapster;
using Skeleton.Application.Feature.Payment.PaymentDto;

namespace Skeleton.Application.Feature.Payment.Mapping;

public static class PaymentMapping
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Entities.Payment, PaymentResponseDto>();
    }
}
