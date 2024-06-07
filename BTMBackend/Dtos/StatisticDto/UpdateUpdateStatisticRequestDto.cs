using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.StatisticDto
{
    public class UpdateUpdateStatisticRequestDto
    {
        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid Input")]
        public int Id { get; set; }
        //public string Name { get; set; } = null!;

        [Range(typeof(bool), "false", "true", ErrorMessage = "Invalid Input")]
        public bool Status { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessage = "Invalid Input")]
        public int FakeValue { get; set; } = 0;
        //[Required]
        //[RegularExpression(@"^\d+$", ErrorMessage = "Invalid Input")]
        //public int UserId { get; set; }
    }
}
