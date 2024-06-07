namespace BTMBackend.Dtos.NotificationDto
{
    public class CreateNotificationRequestDto
    {
        public string ContentAr { get; set; } = null!;
        public string ContentEn { get; set; } = null!;
        public int ReceiverId { get; set; }
    }
}
