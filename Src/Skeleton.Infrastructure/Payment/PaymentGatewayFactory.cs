using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Infrastructure.Payment;

public class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly IEnumerable<IPaymentGatewayService> _gateways;

    public PaymentGatewayFactory(IEnumerable<IPaymentGatewayService> gateways)
        => _gateways = gateways;

    public IPaymentGatewayService GetGateway(PaymentGateway gateway)
        => _gateways.FirstOrDefault(g => g.Gateway == gateway)
           ?? throw new BusinessException(ErrorType.Validation,
               $"Payment gateway '{gateway}' is not supported or not configured.");
}
