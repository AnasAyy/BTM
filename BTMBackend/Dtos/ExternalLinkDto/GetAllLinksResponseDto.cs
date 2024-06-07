namespace BTMBackend.Dtos.ExternalLinkDto
{
    public class GetAllLinksResponseDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public Uri Link { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = null!;
    }
}
