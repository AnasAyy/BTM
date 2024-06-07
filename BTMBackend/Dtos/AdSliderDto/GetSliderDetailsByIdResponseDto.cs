using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Dtos.AdSliderDto
{
    public class GetSliderDetailsByIdResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public byte[]? ImageAr { get; set; }
        public byte[]? ImageEn { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public string UserName { get; set; } = null!;
    }
}
