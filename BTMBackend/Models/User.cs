using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password  { get; set; } = null!;
        [Required]
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;
        public DateTime? LastLoginDateTime { get; set; }
        [Required]
        public DateTime	CreatedAt { get; set; } = DateTime.Now;
        public DateTime	UpdatedAt { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }

        public ICollection<FCMtoken> FCMtokens { get; set; } = null!;
        public ICollection<Customer> Customers { get; set; } = null!;
        public ICollection<Employee>  Employees { get; set; } = null!;
        public ICollection<ExternalLink> ExternalLinks { get; set; } = null!;
        public ICollection<Statistic> Statistics { get; set; } = null!;
        public ICollection<Category> Categories { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = null!;
        public ICollection<Part> Parts { get; set; } = null!;
        public ICollection<AboutUs> AboutUs { get; set; } = null!;
        public ICollection<AdSlider> AdSliders { get; set; } = null!;

    }
}
