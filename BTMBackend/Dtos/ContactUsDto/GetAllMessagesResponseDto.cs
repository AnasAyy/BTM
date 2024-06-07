using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.ContactUsDto
{
    public class GetAllMessagesResponseDto
    {
        public int Id { get; set; }
        public string TypeAR { get; set; } = null!;
        public string TypeEN { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
