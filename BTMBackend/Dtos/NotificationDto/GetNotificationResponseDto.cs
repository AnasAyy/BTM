namespace BTMBackend.Dtos.NotificationDto
{
    public class GetNotificationResponseDto
    {
        public int NumberOfNewNotifications { get; set; }
        public List<NotificationDto> Notifications { get; set; } = null!;
    }
    public class NotificationDto
    {
        public int Id { get; set; }
        public string ContentAr { get; set; } = null!;
        public string ContentEn { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public string CreatedDateTime { get; set; } = null!;
    }
}
