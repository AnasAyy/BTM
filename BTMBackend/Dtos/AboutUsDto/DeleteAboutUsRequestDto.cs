using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.AboutUsDto
{
    public class DeleteAboutUsRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}
