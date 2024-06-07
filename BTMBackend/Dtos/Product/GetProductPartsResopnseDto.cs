namespace BTMBackend.Dtos.Product
{
    public class GetProductPartsResopnseDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public bool Status { get; set; } = true;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserName { get; set; }=null!;
    }
}
