using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Models
{
    public class ContactUs
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Type { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
        public string? Message { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}
