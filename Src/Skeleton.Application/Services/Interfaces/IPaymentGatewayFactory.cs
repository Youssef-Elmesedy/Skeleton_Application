using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.Interfaces;

public interface IPaymentGatewayFactory
{
    IPaymentGatewayService GetGateway(PaymentGateway gateway);
}
