namespace BTMBackend.Dtos.AdSliderDto
{
    public class UpdateAdSliderRequestDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public IFormFile FileAr { get; set; } = null!;
        public IFormFile FileEn { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
