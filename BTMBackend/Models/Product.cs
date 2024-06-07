using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;
        public string? DescriptionAR { get; set; } = null!;
        public string? DescriptionEN { get; set; } = null!;
        [Required]
        public bool HasOffer { get; set; }=false;
        [Required]
        public decimal Price { get; set; } = 0;
        public decimal? OfferPrice { get; set; } = 0;
        public int PurchaseTime { get; set; } = 0;

        [Required]
        public double WarrantyDuration { get; set; } = 0.0;
        public string? ManufacturingCountry { get; set; }
        [Required]
        public string ImageUrl { get; set; } = null!;
        public string? Model { get; set; }
        public string? Brand { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }=DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [ForeignKey("CategoryId")]
        public int? CategoryId { get; set; }

        public ICollection<Part> Parts { get; set; } = null!;
        public ICollection<Accessories_Features> Accessories_Features { get; set; } = null!;
        public ICollection<CustomerProduct> CustomerProducts { get; set; } = null!;

    }
}
