using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class AboutUs
    {
        [Key]
        public int Id { get; set; }
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;
        [Required]
        public string DescriptionAr { get; set; }= null!;
        [Required]
        public string DescriptionEn { get; set; }= null!;
        [Required]
        public bool IsActive { get; set; } = true;
        [Required]
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        [ForeignKey("UserId")]
        public int? UserId { get; set; }
    }
}
