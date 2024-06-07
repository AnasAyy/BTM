namespace BTMBackend.Dtos.AdSliderDto
{
    public class UpdateSliderRequestDto
    {
        public int SliderId { get; set; }
        public string Title { get; set; } = null!;
        public IFormFile FileAr { get; set; } = null!;
        public IFormFile FileEn { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
