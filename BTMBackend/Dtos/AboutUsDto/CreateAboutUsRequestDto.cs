using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.AboutUsDto
{
    public class CreateAboutUsRequestDto
    {
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;
        [Required]
        public string DescriptionAr { get; set; } = null!;
        [Required]
        public string DescriptionEn { get; set; } = null!; 
    }
}
