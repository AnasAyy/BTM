namespace BTMBackend.Dtos.Product
{
    public class GetAllProductResponceDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public bool HasOffer { get; set; }
        public string CategoryNameAr { get; set; } = null!;
        public string CategoryNameEn { get; set; } = null!;


    }
}
