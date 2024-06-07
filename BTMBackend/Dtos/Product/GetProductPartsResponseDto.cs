using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.Product
{
    public class GetProductPartsResponseDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
    }
}
