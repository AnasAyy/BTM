using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Dtos.PartDto
{
    public class UpdatePartRequestDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;
        [Required]
        public string? DescriptionAr { get; set; }
        [Required]
        public string? DescriptionEn { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
        public bool Status { get; set; } = true;
        [Required]
        public int ProductId { get; set; }
    }
}
