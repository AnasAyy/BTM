using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BTMBackend.Models
{
    public class WhoWeAre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DescriptionAr { get; set; } = null!;
        public string DescriptionEn { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}
