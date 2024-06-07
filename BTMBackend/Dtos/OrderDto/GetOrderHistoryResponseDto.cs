namespace BTMBackend.Dtos.OrderDto
{
    public class GetOrderHistoryResponseDto
    {
        public DateTime CreatedAt { get; set; }
        public string StatusAr { get; set; } = null!;
        public string StatusEn { get; set; } = null!;
        public string Employee { get; set; } = null!;
        public string Note { get; set; } = null!;
    }
}
