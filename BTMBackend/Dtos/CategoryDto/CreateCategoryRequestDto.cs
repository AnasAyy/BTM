using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.CategoryDto
{
    public class CreateCategoryRequestDto
    {
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;

    }
}
