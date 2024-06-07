using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class Accessories_Features
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;
        [Required]
        public int Type { get; set; } // 1 Accessories - 2 Features
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }


    }
}
