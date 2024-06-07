using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.PublicListDto
{
    public class GetAllItemsResponseDto
    {
        public int Id { get; set; }
        public string NameAR { get; set; } = null!;
        public string NameEN { get; set; } = null!;
        public string Code { get; set; } = null!;

        public bool Status { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
