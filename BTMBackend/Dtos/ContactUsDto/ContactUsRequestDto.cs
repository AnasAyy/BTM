using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.ContactUsDto
{
    public class ContactUsRequestDto
    {
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid Input")]
        public int Type { get; set; }
        [Required]
        [RegularExpression(@"^[\p{L}0-9\s'-]+$", ErrorMessage = "Invalid Input")]
        public string Name { get; set; } = null!;
        [Required]
        [RegularExpression(@"^[\d\s\-+]{9,15}$", ErrorMessage = "Invalid Input")]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        [RegularExpression(@"^[\p{L}0-9\s\-\+]*$", ErrorMessage = "Invalid Input")]
        public string Message { get; set; }= null!;
    }
}
