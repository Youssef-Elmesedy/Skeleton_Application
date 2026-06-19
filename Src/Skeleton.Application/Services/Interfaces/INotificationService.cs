using Skeleton.Application.Feature.Notification.NotificationDto;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.Interfaces;

public interface INotificationService
{
    // ── Read ─────────────────────────────────────────────────────
    Task<IReadOnlyList<NotificationResponseDto>> GetUserNotificationsAsync(Guid userId, CancellationToken ct);
    Task<PagedResult<NotificationResponseDto>> GetPagedAsync(Guid userId, int page, int pageSize, CancellationToken ct);
    Task<NotificationCountDto> GetUnreadCountAsync(Guid userId, CancellationToken ct);

    // ── Write ─────────────────────────────────────────────────────
    Task<NotificationResponseDto> CreateAsync(CreateNotificationDto dto, CancellationToken ct);
    Task MarkReadAsync(Guid userId, List<Guid> ids, CancellationToken ct);
    Task MarkAllReadAsync(Guid userId, CancellationToken ct);
    Task DeleteAsync(Guid userId, Guid notificationId, CancellationToken ct);

    // ── Broadcast helpers ─────────────────────────────────────────
    Task NotifyOrderStatusAsync(Guid customerId, Guid orderId, OrderStatus status, CancellationToken ct);
    Task NotifyPaymentAsync(Guid customerId, Guid paymentId, PaymentStatus status, CancellationToken ct);
    Task NotifyLowStockAsync(Guid productId, string productName, decimal remaining, CancellationToken ct);
}
