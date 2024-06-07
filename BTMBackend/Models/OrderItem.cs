using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ItemType { get; set; } // 1-Product 2-Part

        [Required]
        public int ServiceType { get; set; } // 1-New // 2-Maintenance //3-Other

        [Required]
        public int ItemId { get; set; }

        public int CustomerPartId { get; set; } = 0; //في حالة كان المنتج صيانة دورية

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("OrderId")]
        public int? OrderId { get; set; }

    }
}
