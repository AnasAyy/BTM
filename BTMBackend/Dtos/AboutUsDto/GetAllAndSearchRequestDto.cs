using BTMBackend.Utilities;
using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.AboutUsDto
{
    public class GetAllAndSearchRequestDto
    {
        [Required]
        public int Page { get; set; }
        public string? Name { get; set; }
    }
}
