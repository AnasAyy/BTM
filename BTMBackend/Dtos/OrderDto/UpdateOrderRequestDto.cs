namespace BTMBackend.Dtos.OrderDto
{
    public class UpdateOrderRequestDto
    {
        public int OrderId { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
