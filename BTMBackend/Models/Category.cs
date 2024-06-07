using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BTMBackend.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string NameAr { get; set; } = null!; // change to Name Ar
        public string NameEn { get; set; } = null!; // Added
        public bool IsActive { get; set; } = true; // Changed from Status to IsActive
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public ICollection<Product> Products { get; set; } = null!;
    }
}
