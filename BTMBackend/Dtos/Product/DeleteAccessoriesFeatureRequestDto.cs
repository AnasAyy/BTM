using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.Product
{
    public class DeleteAccessoriesFeatureRequestDto
    {
        [Required]
        public int  Id { get; set; }
    }
}
