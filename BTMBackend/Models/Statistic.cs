using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class Statistic
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        
        public int RealValue { get; set; } = 0;
        public int FakeValue { get; set; } = 0;
        [Required]
        public bool Status { get; set; } = true;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

    }
}
