using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class OrderStatusHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Status { get; set; } // 0-UnderReviewFromSuperviser 1-SentToCallCenter 2-SentToTechnician 3-PrepaireToCancel 4-Canceled 5-Done
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("OrderId")]
        public int? OrderId { get; set; }
        [ForeignKey("EmployeeId")]
        public int? EmployeeId { get; set; }

    }
}
