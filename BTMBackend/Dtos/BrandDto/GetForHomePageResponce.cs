namespace BTMBackend.Dtos.BrandDto
{
    public class GetForHomePageResponce
    {
        public int Id { get; set; }
        public string Logo { get; set; } = null!;
        public byte[] Photo { get; set; } = null!;
    }
}
