namespace BTMBackend.Dtos.OrderDto
{
    public class UpdateMultiOrderRequestDto
    {
        public int[] Id { get; set; } = null!;
        public string Note { get; set; } = string.Empty;
        public int? FromEmployeeId { get; set; }
        public int ToEmployeeId { get; set; }

    }
}
