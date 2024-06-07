namespace BTMBackend.Dtos.AdSliderDto
{
    public class GetAllSliderResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
