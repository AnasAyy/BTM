using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.Product
{
    public class UpdateProductRequestDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;
        [Required]
        public string? DescriptionAR { get; set; } = null!;
        [Required]
        public string? DescriptionEN { get; set; } = null!;
        [Required]
        public bool HasOffer { get; set; }
        [Required]
        public decimal? Price { get; set; }
        [Required]
        public decimal? OfferPrice { get; set; }
        [Required]
        public double? WarrantyDuration { get; set; }
        [Required]
        public string? ManufacturingCountry { get; set; }
        public IFormFile? File { get; set; } = null!;
        [Required]
        public string? Model { get; set; }
        [Required]
        public string? Brand { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        [Required]
        public int CategoryId { get; set; }

    }
}
