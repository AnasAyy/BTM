using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.ExternalLinkDto
{
    public class GetLinkByNameRequestDto
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
