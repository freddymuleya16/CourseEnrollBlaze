using CourseEnrollBlaze.Server.Services;
using CourseEnrollBlaze.Shared.Models;

namespace CourseEnrollBlaze.Server.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetNotificationsAsync(string recipientId);
        Task<List<Notification>> GetNotificationsForRecipientAsync(string recipientId);
        Task<Notification> GetNotificationAsync(Guid id);
        Task AddNotificationAsync(string title, string message, string recipientId, string link = null);
        Task MarkAsReadAsync(Guid id);
        event EventHandler<NotificationEventArgs> NotificationAdded;
        event EventHandler<NotificationEventArgs> NotificationUpdated;

        Task UpdateNotificationAsync(Notification notification);
    }
}
