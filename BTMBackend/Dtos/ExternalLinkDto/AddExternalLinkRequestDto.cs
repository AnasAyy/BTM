using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.ExternalLinkDto
{
    public class AddExternalLinkRequestDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [RegularExpression(@"^https?://[\w\-]+(\.[\w\-]+)+[/#?]?.*$", ErrorMessage = "Invalid Input")]
        public Uri Link { get; set; } = null!;
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid Input")]
        public int Type { get; set; }
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid Input")]
        public int UserId { get; set; }
    }
}
