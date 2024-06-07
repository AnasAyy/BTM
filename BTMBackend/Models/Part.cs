using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class Part
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }

        public double MaintenanceDuration { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool Status { get; set; } = true;

        [ForeignKey("UserId")]
        public int? UserId { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }

        public ICollection<CustomerProductPart> CustomerProductParts { get; set; } = null!;


    }
}
