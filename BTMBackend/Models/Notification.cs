using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string ContentAr { get; set; } = null!;
        public string ContentEn { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public int ReceiverId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ReadAt { get; set; }
    }
}
