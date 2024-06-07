using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public int City { get; set; }
        
        [Required]
        public int County { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        public int? SupervisorId  { get; set; }
        [ForeignKey("SupervisorId ")]
        public virtual User Supervisor { get; set; } = null!;

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public ICollection<Order> Orders { get; set; } = null!;
        public ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;
    }
}
