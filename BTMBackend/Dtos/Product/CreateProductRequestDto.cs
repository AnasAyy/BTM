using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.Product
{
    public class CreateProductRequestDto
    {
        [Required]
        public string NameAr { get; set; } = null!;
        [Required]
        public string NameEn { get; set; } = null!;
        public string? DescriptionAR { get; set; } = null!;
        public string? DescriptionEN { get; set; } = null!;
        [Required]
        public decimal Price { get; set; } = 0;
        [Required]
        public double WarrantyDuration { get; set; } = 0.0;
        public string? ManufacturingCountry { get; set; }
        [Required]
        public IFormFile File { get; set; } = null!;
        public string? Model { get; set; }
        public string? Brand { get; set; }
        [Required]
        public int CategoryId { get; set; }

        //public List<AccessoriesFeature> Accessories { get; set; }= null!;
        //public List<AccessoriesFeature> Features { get; set; }= null!;
        
    }

    //public class AccessoriesFeature
    //{
    //    public string NameAr { get; set; } = string.Empty;
    //    public string NameEn { get; set; } = string.Empty;
    //}
}
