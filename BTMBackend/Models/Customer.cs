using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string Address { get; set; } = null!;
        public int City { get; set; }
        public int County { get; set; }
        [Required]
        public int TotalPurchasesAmount { get; set; } = 0;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public ICollection<Order> Orders { get; set; } = null!;
        public ICollection<CustomerProduct> CustomerProducts { get; set; } = null!;

    }
}
