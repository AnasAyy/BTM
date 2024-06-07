using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Models
{
    public class PublicList
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string NameAR { get; set; } = null!;
        [Required]
        public string NameEN { get; set; } = null!;
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public bool Status { get; set; } = true;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        [Required]
        public int Type { get; set; } = 1;

    }
}
