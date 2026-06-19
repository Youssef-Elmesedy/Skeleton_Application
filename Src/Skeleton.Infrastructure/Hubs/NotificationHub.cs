using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Skeleton.Infrastructure.Hubs;

/// <summary>
/// Real-time notification hub via WebSocket / SignalR.
///
/// Client connects to: /hubs/notifications
/// Client must send JWT: ?access_token={token} or Authorization: Bearer {token}
///
/// JS example:
///   const conn = new signalR.HubConnectionBuilder()
///     .withUrl("/hubs/notifications", { accessTokenFactory: () => token })
///     .withAutomaticReconnect()
///     .build();
///   conn.on("ReceiveNotification", (notification) => { ... });
///   await conn.start();
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private static readonly Dictionary<string, string> _userConnections = new();

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId is not null)
        {
            _userConnections[userId] = Context.ConnectionId;
            // Add to user-specific group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (userId is not null)
            _userConnections.Remove(userId);

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>Client can call this to mark notification read.</summary>
    public async Task MarkRead(Guid notificationId)
    {
        await Clients.Caller.SendAsync("NotificationMarkedRead", notificationId);
    }

    /// <summary>Admin broadcasts message to all connected users.</summary>
    public async Task BroadcastAlert(string title, string message)
    {
        if (Context.User?.IsInRole("Admin") == true)
            await Clients.All.SendAsync("BroadcastAlert", new { title, message });
    }
}
