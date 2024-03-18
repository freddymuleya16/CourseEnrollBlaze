using CourseEnrollBlaze.Server.Interfaces;
using CourseEnrollBlaze.Server.Services;
using CourseEnrollBlaze.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace CourseEnrollBlaze.Server.Notifications
{
    public class NotificationSender : BackgroundService
    {
        private readonly ILogger<NotificationSender> _logger;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private readonly IServiceScopeFactory _serviceScopeFactory; 

        public NotificationSender(ILogger<NotificationSender> logger,
            IHubContext<NotificationHub, INotificationClient> hubContext,
            IServiceScopeFactory serviceScopeFactory )
        {
            _logger = logger;
            _hubContext = hubContext;
            _serviceScopeFactory = serviceScopeFactory;  
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                 
                _notificationService.NotificationAdded += async (sender, args) => await OnNotificationAdded(sender, args);
                _notificationService.NotificationUpdated += async (sender, args) => await OnNotificationUpdated(sender, args);


                return base.StartAsync(cancellationToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                 
                _notificationService.NotificationAdded -= async (sender, args) => await OnNotificationAdded(sender, args);
                _notificationService.NotificationUpdated -= async (sender, args) => await OnNotificationUpdated(sender, args);


                return base.StopAsync(cancellationToken);
            }
        }
          
        private async Task OnNotificationAdded(object sender, NotificationEventArgs e)
        {
            await SendNotificationToClient(e.Notifications);
        }

        private async Task OnNotificationUpdated(object sender, NotificationEventArgs e)
        {
            await SendNotificationToClient(e.Notifications);
        }

        private async Task SendNotificationToClient(Notification[] notifications)
        {
            try
            { 
                var groupedNotifications = notifications.GroupBy(n => n.RecipientId);

                foreach (var group in groupedNotifications)
                { 
                    var recipientId = group.Key;
                     
                    var notificationsArray = group.ToArray();
                     
                    await _hubContext.Clients.User(recipientId).ReceivedNotification(notificationsArray);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending notifications to clients.");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        { 
            return Task.CompletedTask;
        }
    }
}
