using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.CategoryDto
{
    public class UpdateCategoryRequestDto
    {
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid Input")]
        public int Id { get; set; }
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        [Range(typeof(bool), "false", "true", ErrorMessage = "Invalid Input")]
        public bool IsActive { get; set; }
        
    }
}
