using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.BrandDto
{
    public class CreateBrandRequestDto
    {
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]

        public string NameEn { get; set; } = null!;
        [Required]

        public IFormFile File { get; set; } = null!;
    }
}
