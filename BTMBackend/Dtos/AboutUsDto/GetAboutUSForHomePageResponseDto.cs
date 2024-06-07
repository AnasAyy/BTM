namespace BTMBackend.Dtos.AboutUsDto
{
    public class GetAboutUSForHomePageResponseDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = null!;
        public string DescriptionAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string DescriptionEn { get; set; } = null!;
    }
}
