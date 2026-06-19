using Skeleton.Domain.Eunm;

namespace Skeleton.Domain.Entities;

/// <summary>
/// Persistent notification record — stored in DB, served to client via API or SignalR.
/// </summary>
public class Notification : BaseEntity
{
    public Guid?            UserId           { get; private set; }  // null = broadcast
    public Guid?            CustomerId       { get; private set; }
    public Guid?            EmployeeId       { get; private set; }
    public NotificationType Type             { get; private set; }
    public string           Title            { get; private set; } = null!;
    public string           Message          { get; private set; } = null!;
    public string?          ActionUrl        { get; private set; }  // deep-link
    public string?          IconClass        { get; private set; }  // CSS icon key
    public bool             IsRead           { get; private set; } = false;
    public DateTime?        ReadAt           { get; private set; }
    public bool             IsSentEmail      { get; private set; } = false;
    public bool             IsSentPush       { get; private set; } = false;
    public string?          Metadata         { get; private set; }  // JSON blob

    private Notification() { }  // EF

    public Notification(
        NotificationType type, string title, string message,
        Guid? userId = null, Guid? customerId = null, Guid? employeeId = null,
        string? actionUrl = null, string? metadata = null)
    {
        Type       = type;
        Title      = title.Trim();
        Message    = message.Trim();
        UserId     = userId;
        CustomerId = customerId;
        EmployeeId = employeeId;
        ActionUrl  = actionUrl;
        Metadata   = metadata;
        IconClass  = ResolveIcon(type);
    }

    public void MarkRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }

    public void MarkEmailSent()   => IsSentEmail = true;
    public void MarkPushSent()    => IsSentPush  = true;

    private static string ResolveIcon(NotificationType type) => type switch
    {
        NotificationType.OrderCreated    => "shopping-bag",
        NotificationType.OrderConfirmed  => "check-circle",
        NotificationType.OrderShipped    => "truck",
        NotificationType.OrderDelivered  => "package",
        NotificationType.OrderCancelled  => "x-circle",
        NotificationType.PaymentReceived => "credit-card",
        NotificationType.PaymentFailed   => "alert-circle",
        NotificationType.PaymentRefunded => "corner-up-left",
        NotificationType.AccountDebit    => "trending-up",
        NotificationType.AccountPayment  => "trending-down",
        NotificationType.LowStock        => "alert-triangle",
        NotificationType.SystemAlert     => "bell",
        _                                => "bell"
    };
}
