using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.AboutUsDto
{
    public class SearchAboutUsRequestDto
    {

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Type must be a positive integer.")]
        public int Page { get; set; }
        public string Name { get; set; } = null!;
    }
}
