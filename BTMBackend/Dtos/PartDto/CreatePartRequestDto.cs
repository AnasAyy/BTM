using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.PartDto
{
    public class CreatePartRequestDto
    {
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
