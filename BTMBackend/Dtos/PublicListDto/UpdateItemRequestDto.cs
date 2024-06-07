using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.PublicListDto
{
    public class UpdateItemRequestDto
    {
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Type must be a positive integer.")]
        public int Id { get; set; }
        [Required]

        [RegularExpression(@"^[\p{L}0-9'-]+$", ErrorMessage = "Invalid Input")]
        public string NameAR { get; set; } = null!;

        [Required]

        [RegularExpression(@"^[A-Za-z0-9' -]+$", ErrorMessage = "Invalid Input")]
        public string NameEN { get; set; } = null!;
        [Required]

        [Range(typeof(bool), "false", "true", ErrorMessage = "Invalid Input")]
        public bool Status { get; set; }
        [Required]
        public string Code { get; set; } = null!;
        //public int Type {  get; set; }

        
        
    }
}
