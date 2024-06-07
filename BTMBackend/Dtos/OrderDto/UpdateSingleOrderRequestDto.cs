namespace BTMBackend.Dtos.OrderDto
{
    public class UpdateSingleOrderRequestDto
    {
        public int OrderId { get; set; }
        public string Note { get; set; } = string.Empty;
        public int? FromEmployeeId { get; set; }
        public int? ToEmployeeId { get; set; }
        public int Status { get; set; }
    }
}
