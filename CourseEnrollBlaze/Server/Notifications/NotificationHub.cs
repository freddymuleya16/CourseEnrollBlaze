using CourseEnrollBlaze.Server.Interfaces;
using CourseEnrollBlaze.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CourseEnrollBlaze.Server.Notifications
{
    [Authorize]
    public class NotificationHub : Hub<INotificationClient>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public NotificationHub(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task OnConnectedAsync()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var user = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (user != null)
                {
                    var notifications = await _notificationService.GetNotificationsAsync(user);
                    await Clients.Client(Context.ConnectionId).ReceivedNotification(notifications.ToArray<Notification>());
                }

            }
            await base.OnConnectedAsync();
        }
    }
    public interface INotificationClient
    {
        Task ReceivedNotification(Notification[] notifications);
    }
}

