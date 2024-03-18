using CourseEnrollBlaze.Server.Context;
using CourseEnrollBlaze.Server.Interfaces;
using CourseEnrollBlaze.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseEnrollBlaze.Server.Services
{

    public class NotificationEventArgs : EventArgs
    {
        public Notification[] Notifications { get; }

        public NotificationEventArgs(Notification[] notifications)
        {
            Notifications = notifications;
        }
    }

    public class NotificationService : INotificationService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public event EventHandler<NotificationEventArgs> NotificationAdded;
        public event EventHandler<NotificationEventArgs> NotificationUpdated;



        public NotificationService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

        }


        public async Task<IEnumerable<Notification>> GetNotificationsAsync(string recipientId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                CEBDbContext _context = scope.ServiceProvider.GetRequiredService<CEBDbContext>();

                return await _context.Notifications.Where(n => n.RecipientId == recipientId).ToListAsync();
            }
        }
        public async Task<List<Notification>> GetNotificationsForRecipientAsync(string recipientId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                CEBDbContext _context = scope.ServiceProvider.GetRequiredService<CEBDbContext>();

                return await _context.Notifications.Where(n => n.RecipientId == recipientId).ToListAsync();
            }
        }

        public async Task<Notification> GetNotificationAsync(Guid id)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                CEBDbContext _context = scope.ServiceProvider.GetRequiredService<CEBDbContext>();

                return await _context.Notifications.FindAsync(id)??new Notification();
            }
        }

        public async Task AddNotificationAsync(string title, string message, string recipientId, string? link = null)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                CEBDbContext _context = scope.ServiceProvider.GetRequiredService<CEBDbContext>();

                var notification = new Notification
                {
                    Title = title,
                    Message = message,
                    RecipientId = recipientId,
                    Link = link,
                    Timestamp = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                var nots = await _context.Notifications.ToArrayAsync();
                OnNotificationAdded(new NotificationEventArgs(nots));
            }
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                CEBDbContext _context = scope.ServiceProvider.GetRequiredService<CEBDbContext>();

                var notification = await _context.Notifications.FindAsync(id);
                if (notification != null)
                {
                    notification.IsRead = true;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                CEBDbContext _context = scope.ServiceProvider.GetRequiredService<CEBDbContext>();

                _context.Entry(notification).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                var nots = await _context.Notifications.ToArrayAsync();

                OnNotificationUpdated(new NotificationEventArgs(nots));
            }
        }

        protected virtual void OnNotificationAdded(NotificationEventArgs e)
        {
            if (NotificationAdded != null)
            {
                foreach (Delegate subscriber in NotificationAdded.GetInvocationList())
                {
                    Console.WriteLine($"Subscriber method name: {subscriber.Method.Name}");
                }
            }

            NotificationAdded?.Invoke(this, e);
        }

        protected virtual void OnNotificationUpdated(NotificationEventArgs e)
        {
            NotificationUpdated?.Invoke(this, e);
        }
    }
}
