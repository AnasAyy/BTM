using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.PublicListDto
{
    public class AddItemRequestDto
    {
        [Required]
        [RegularExpression(@"^[\p{L}0-9\s'-]+$", ErrorMessage = "Invalid Input")]
        public string NameAR { get; set; } = null!;

        [Required]
        [RegularExpression(@"^[A-Za-z0-9' -]+$", ErrorMessage = "Invalid Input")]
        public string NameEN { get; set; } = null!;
        
        [Required]
        public string Code { get; set; } = null!;

        
    }
}
