using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Feature.Payment.PaymentDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.Implementation.Queries;

// ════════════════════════════════════════
//  Payment Query Repository
// ════════════════════════════════════════
internal class PaymentQueryRepository : IPaymentQueryRepository
{
    private readonly AppDbContext _context;
    public PaymentQueryRepository(AppDbContext context) => _context = context;

    public async Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Payments
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PaymentResponseDto(
                p.Id, p.OrderId, p.Amount, p.GatewayFee, p.NetAmount,
                p.Method, p.Gateway, p.Status, p.GatewayStatus, p.GatewayTransactionId,
                p.GatewayReferenceCode, p.FailureReason, p.RefundReason, p.RefundAmount, p.IsInstallment,
                p.InstallmentMonths, p.MonthlyAmount, p.InitiatedAt, p.CompletedAt, p.RefundedAt, p.PaidAt,
                p.CreateDate))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentResponseDto>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await _context.Payments
            .AsNoTracking()
            .Where(p => p.OrderId == orderId)
            .Select(p => new PaymentResponseDto(
                p.Id, p.OrderId, p.Amount, p.GatewayFee, p.NetAmount,
                p.Method, p.Gateway, p.Status, p.GatewayStatus, p.GatewayTransactionId,
                p.GatewayReferenceCode, p.FailureReason, p.RefundReason, p.RefundAmount, p.IsInstallment,
                p.InstallmentMonths, p.MonthlyAmount, p.InitiatedAt, p.CompletedAt, p.RefundedAt, p.PaidAt,
                p.CreateDate))
            .ToListAsync(cancellationToken);
    }
}
