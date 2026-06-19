using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Common;
using Skeleton.Application.Feature.Notification.NotificationDto;
using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;
using Skeleton.Infrastructure.Hubs;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _ctx;
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationService(AppDbContext ctx, IHubContext<NotificationHub> hub)
    {
        _ctx = ctx;
        _hub = hub;
    }

    // ─────────────────────────────────────────────────────────────
    //  Read
    // ─────────────────────────────────────────────────────────────
    public async Task<IReadOnlyList<NotificationResponseDto>> GetUserNotificationsAsync(
        Guid userId, CancellationToken ct)
        => await ProjectQuery(userId).OrderByDescending(n => n.CreateDate).ToListAsync(ct);

    public async Task<PagedResult<NotificationResponseDto>> GetPagedAsync(
        Guid userId, int page, int pageSize, CancellationToken ct)
    {
        var query = ProjectQuery(userId).OrderByDescending(n => n.CreateDate);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<NotificationResponseDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };
    }

    public async Task<NotificationCountDto> GetUnreadCountAsync(Guid userId, CancellationToken ct)
    {
        var total = await _ctx.Notifications.CountAsync(n => n.UserId == userId, ct);
        var unread = await _ctx.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct);
        return new NotificationCountDto(total, unread);
    }

    // ─────────────────────────────────────────────────────────────
    //  Write
    // ─────────────────────────────────────────────────────────────
    public async Task<NotificationResponseDto> CreateAsync(CreateNotificationDto dto, CancellationToken ct)
    {
        var notification = new Notification(
            dto.Type, dto.Title, dto.Message,
            dto.UserId, dto.CustomerId, dto.EmployeeId,
            dto.ActionUrl, dto.Metadata);

        _ctx.Notifications.Add(notification);
        await _ctx.SaveChangesAsync(ct);

        var response = ToDto(notification);

        // Push real-time via SignalR
        if (dto.UserId.HasValue)
            await _hub.Clients.User(dto.UserId.Value.ToString())
                      .SendAsync("ReceiveNotification", response, ct);

        return response;
    }

    public async Task MarkReadAsync(Guid userId, List<Guid> ids, CancellationToken ct)
    {
        var notifications = await _ctx.Notifications
            .Where(n => n.UserId == userId && ids.Contains(n.Id) && !n.IsRead)
            .ToListAsync(ct);

        foreach (var n in notifications) n.MarkRead();
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task MarkAllReadAsync(Guid userId, CancellationToken ct)
    {
        var unread = await _ctx.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(ct);
        foreach (var n in unread) n.MarkRead();
        await _ctx.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid userId, Guid notificationId, CancellationToken ct)
    {
        var n = await _ctx.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, ct)
            ?? throw new BusinessException(ErrorType.NotFound, "Notification not found.");
        _ctx.Notifications.Remove(n);
        await _ctx.SaveChangesAsync(ct);
    }

    // ─────────────────────────────────────────────────────────────
    //  Domain-specific helpers
    // ─────────────────────────────────────────────────────────────
    public async Task NotifyOrderStatusAsync(
        Guid customerId, Guid orderId, OrderStatus status, CancellationToken ct)
    {
        var (title, message, type) = status switch
        {
            OrderStatus.Confirmed => ("Order Confirmed ✅", $"Your order has been confirmed.", NotificationType.OrderConfirmed),
            OrderStatus.Processing => ("Order Processing ⚙️", $"Your order is being processed.", NotificationType.OrderConfirmed),
            OrderStatus.Shipped => ("Order Shipped 🚚", $"Your order is on its way!", NotificationType.OrderShipped),
            OrderStatus.Delivered => ("Order Delivered 📦", $"Your order has been delivered.", NotificationType.OrderDelivered),
            OrderStatus.Cancelled => ("Order Cancelled ❌", $"Your order has been cancelled.", NotificationType.OrderCancelled),
            _ => ("Order Update", $"Your order status changed.", NotificationType.OrderConfirmed)
        };

        // Find AppUser by CustomerId
        var user = await _ctx.AppUsers.FirstOrDefaultAsync(u => u.CustomerId == customerId, ct);
        if (user is null) return;

        await CreateAsync(new CreateNotificationDto(
            type, title, message,
            user.Id, customerId, null,
            $"/orders/{orderId}"), ct);
    }

    public async Task NotifyPaymentAsync(
        Guid customerId, Guid paymentId, PaymentStatus status, CancellationToken ct)
    {
        var (title, message, type) = status switch
        {
            PaymentStatus.Completed => ("Payment Received 💳", "Your payment was successful.", NotificationType.PaymentReceived),
            PaymentStatus.Failed => ("Payment Failed ❌", "Your payment could not be processed.", NotificationType.PaymentFailed),
            PaymentStatus.Refunded => ("Refund Processed 💸", "Your refund has been issued.", NotificationType.PaymentRefunded),
            _ => ("Payment Update", "Payment status updated.", NotificationType.PaymentReceived)
        };

        var user = await _ctx.AppUsers.FirstOrDefaultAsync(u => u.CustomerId == customerId, ct);
        if (user is null) return;

        await CreateAsync(new CreateNotificationDto(
            type, title, message,
            user.Id, customerId, null,
            $"/payments/{paymentId}"), ct);
    }

    public async Task NotifyLowStockAsync(
        Guid productId, string productName, decimal remaining, CancellationToken ct)
    {
        // Notify all Admin users
        var admins = await _ctx.AppUsers
            .Where(u => u.Role == Domain.Eunm.UserRole.Admin)
            .ToListAsync(ct);

        foreach (var admin in admins)
        {
            await CreateAsync(new CreateNotificationDto(
                NotificationType.LowStock,
                "Low Stock Alert ⚠️",
                $"{productName} has only {remaining} units remaining.",
                admin.Id, null, null,
                $"/products/{productId}"), ct);
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  Private helpers
    // ─────────────────────────────────────────────────────────────
    private IQueryable<NotificationResponseDto> ProjectQuery(Guid userId)
        => _ctx.Notifications
               .AsNoTracking()
               .Where(n => n.UserId == userId)
               .Select(n => new NotificationResponseDto(
                   n.Id, n.Type, n.Title, n.Message,
                   n.ActionUrl, n.IconClass,
                   n.IsRead, n.ReadAt, n.CreateDate));

    private static NotificationResponseDto ToDto(Notification n) => new(
        n.Id, n.Type, n.Title, n.Message,
        n.ActionUrl, n.IconClass, n.IsRead, n.ReadAt, n.CreateDate);
}
