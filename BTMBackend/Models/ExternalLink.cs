using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class ExternalLink
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;
        [Required]
        [Url]
        public Uri Link { get; set; }=null!;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt {  get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
    }
}
