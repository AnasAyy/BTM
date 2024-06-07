using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.PublicDto
{
    public class PagnationRequestDto
    {
        [Required]
        public int Page { get; set; }
        
    }
}
