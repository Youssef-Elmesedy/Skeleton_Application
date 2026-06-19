using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Feature.Notification.NotificationDto;

public record NotificationResponseDto(
    Guid             Id,
    NotificationType Type,
    string           Title,
    string           Message,
    string?          ActionUrl,
    string?          IconClass,
    bool             IsRead,
    DateTime?        ReadAt,
    DateTime?        CreateDate
);

public record NotificationCountDto(int Total, int Unread);

public record MarkReadDto(List<Guid> NotificationIds);

/// <summary>Used internally to create notifications</summary>
public record CreateNotificationDto(
    NotificationType Type,
    string           Title,
    string           Message,
    Guid?            UserId     = null,
    Guid?            CustomerId = null,
    Guid?            EmployeeId = null,
    string?          ActionUrl  = null,
    string?          Metadata   = null
);
