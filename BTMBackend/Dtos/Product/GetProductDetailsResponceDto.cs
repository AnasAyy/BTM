using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.Product
{
    public class GetProductDetailsResponceDto
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string? DescriptionAR { get; set; } 
        public string? DescriptionEN { get; set; } 
        public bool HasOffer { get; set; } = false;
        public decimal Price { get; set; } 
        public decimal? OfferPrice { get; set; } 
        public double WarrantyDuration { get; set; } 
        public string? ManufacturingCountry { get; set; }
        public byte[] Image { get; set; } = null!;
        public string? Model { get; set; }
        public string? Brand { get; set; } 
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }
        public string UserName { get; set; } = null!;
        public int CategoryId { get; set; } 
    }
}
