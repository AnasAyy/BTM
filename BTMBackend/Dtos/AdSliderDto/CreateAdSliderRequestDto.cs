namespace BTMBackend.Dtos.AdSliderDto
{
    public class CreateAdSliderRequestDto
    {
        public string Title { get; set; } = null!;
        public IFormFile FileAr { get; set; } = null!;
        public IFormFile FileEn { get; set; } = null!;
    }
}
