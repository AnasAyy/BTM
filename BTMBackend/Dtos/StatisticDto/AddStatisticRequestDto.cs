using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.StatisticDto
{
    public class AddStatisticRequestDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid Input")]
        public int FakeValue { get; set; } = 0;
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid Input")]
        public int UserId { get; set; }
    }
}
