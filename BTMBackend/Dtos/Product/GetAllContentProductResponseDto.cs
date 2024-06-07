namespace BTMBackend.Dtos.Product
{
    public class GetAllContentProductResponseDto
    {
        public int ProductId { get; set; }
        public byte[]? Image { get; set; }
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string? Model { get; set; }
        public string? Brand { get; set; }
        public string? DescriptionAR { get; set; } = null!;
        public string? DescriptionEN { get; set; } = null!;
        public decimal Price { get; set; } = 0;
        public bool HasOffer { get; set; } = false;
        public decimal? OfferPrice { get; set; } = 0;
        public double WarrantyDuration { get; set; } = 0.0;

    }
}
