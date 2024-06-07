namespace BTMBackend.Dtos.PublicDto
{
    public class UploadFileByTableIdRequestDto
    {
        public int OrderId { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
