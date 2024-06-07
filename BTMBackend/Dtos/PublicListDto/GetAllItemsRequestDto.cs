using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.PublicListDto
{
    public class GetAllItemsRequestDto
    {
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Type must be a positive integer.")]
        public int Page { get; set; }
        [Required]
        public string Code { get; set; } = null!;
    }
}
