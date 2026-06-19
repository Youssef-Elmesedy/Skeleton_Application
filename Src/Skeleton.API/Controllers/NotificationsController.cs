using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Skeleton.Application.Feature.Notification.NotificationDto;
using Skeleton.Application.Services.Interfaces;

namespace Skeleton.Controllers;

/// <summary>
/// Notifications — Real-time and persistent user notifications.
/// </summary>
/// <remarks>
/// Notifications are pushed in real-time via **SignalR** and stored in DB for history.
///
/// ### Connect via SignalR
/// ```js
/// const conn = new signalR.HubConnectionBuilder()
///   .withUrl("/hubs/notifications", { accessTokenFactory: () => yourJwtToken })
///   .withAutomaticReconnect()
///   .build();
///
/// conn.on("ReceiveNotification", (notification) => {
///   console.log("New:", notification.title);
/// });
///
/// await conn.start();
/// ```
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : BaseController
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service) => _service = service;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get all notifications for the current user (paginated).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _service.GetPagedAsync(CurrentUserId, page, pageSize, ct);
        return Ok(ApiResponse<object>.Success("Notifications retrieved.", result));
    }

    /// <summary>Get unread notification count.</summary>
    [HttpGet("count")]
    [ProducesResponseType(typeof(ApiResponse<NotificationCountDto>), 200)]
    public async Task<IActionResult> GetCount(CancellationToken ct)
    {
        var result = await _service.GetUnreadCountAsync(CurrentUserId, ct);
        return Ok(ApiResponse<NotificationCountDto>.Success("Notification count.", result));
    }

    /// <summary>Mark specific notifications as read.</summary>
    [HttpPatch("mark-read")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> MarkRead(
        [FromBody] MarkReadDto dto, CancellationToken ct)
    {
        await _service.MarkReadAsync(CurrentUserId, dto.NotificationIds, ct);
        return Ok(ApiResponse<object>.Success("Notifications marked as read.", null!));
    }

    /// <summary>Mark ALL notifications as read.</summary>
    [HttpPatch("mark-all-read")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        await _service.MarkAllReadAsync(CurrentUserId, ct);
        return Ok(ApiResponse<object>.Success("All notifications marked as read.", null!));
    }

    /// <summary>Delete a notification.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(CurrentUserId, id, ct);
        return Ok(ApiResponse<object>.Success("Notification deleted.", null!));
    }
}
