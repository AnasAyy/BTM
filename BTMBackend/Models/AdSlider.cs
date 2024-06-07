using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class AdSlider
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string ImageArPath { get; set; } = null!;
        public string ImageEnPath { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
    }
}
