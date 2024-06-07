using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.ExternalLinkDto
{
    public class UpdateExternalLinkRequestDto
    {
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid Input")]
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^https?://[\w\-]+(\.[\w\-]+)+[/#?]?.*$", ErrorMessage = "Invalid Input")]
        public Uri Link { get; set; } = null!;
        
    }
}
