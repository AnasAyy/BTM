using BTMBackend.Dtos.NotificationDto;
using BTMBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BTMBackend.Data.Repos
{
    public interface INotificationRepo
    {
        Task<bool> Create(List<Notification> notification);
        Task<bool> Update(List<Notification> notification);
        Task<GetNotificationResponseDto> GetByUserId(int userId);
        Task<List<Notification>> GetByIdList(int[] id);
        Task<bool> CreateSingle(Notification notification);
        Task<bool> ChangeNotificationStatus(int userId);
    }
    public class NotificationRepo(DataContext context) : INotificationRepo
    {
        private readonly DataContext _context = context;

        public async Task<bool> Create(List<Notification> notification)
        {
            await _context.Notifications.AddRangeAsync(notification);
            var result = await SaveChangres();
            return result;
        }

        private async Task<bool> Delete(List<Notification> notification)
        {
            _context.Notifications.RemoveRange(notification);
            var result = await SaveChangres();
            return result;
        }

        public async Task<GetNotificationResponseDto> GetByUserId(int userId)
        {
            var numberOfNewNotifications = await _context.Notifications
                .CountAsync(x => x.ReceiverId == userId && !x.IsRead);

            var notifications = await _context.Notifications
                .Where(x => x.ReceiverId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NotificationDto()
                {
                    Id = x.Id,
                    ContentAr = x.ContentAr,
                    ContentEn = x.ContentEn,
                    IsRead = x.IsRead,
                    CreatedDateTime = x.CreatedAt.ToString("dd/MM/yyyy hh:mm tt")
                })
                .ToListAsync();

            return new GetNotificationResponseDto()
            {
                NumberOfNewNotifications = numberOfNewNotifications,
                Notifications = notifications
            };
        }




        public async Task<bool> Update(List<Notification> notification)
        {
            var oldNotification = await _context.Notifications.Where(x => x.CreatedAt > DateTime.UtcNow.AddMonths(1)).ToListAsync();
            if (oldNotification.Count != 0)
            {
                await Delete(oldNotification);
            }
            _context.Notifications.UpdateRange(notification);
            var result = await SaveChangres();
            return result;
        }

        private async Task<bool> SaveChangres()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public async Task<List<Notification>> GetByIdList(int[] ids)
        {
            var notifications = await _context.Notifications
                .Where(n => ids.Contains(n.Id))
                .ToListAsync();

            return notifications;
        }

        public async Task<bool> ChangeNotificationStatus(int userId)
        {
            var notificationsToUpdate = _context.Notifications
                .Where(n => n.ReceiverId == userId && !n.IsRead);

            foreach (var notification in notificationsToUpdate)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Log the exception or handle it as needed
                return false;
            }
        }

        public async Task<bool> CreateSingle(Notification notification)
        {
            _context.Notifications.Add(notification);
            var result = await SaveChangres();
            return result;
        }
    }
}
