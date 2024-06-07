using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.Product
{
    public class ProductDetailsRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}
