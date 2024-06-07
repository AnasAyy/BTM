namespace BTMBackend.Dtos.OrderDto
{
    public class GetAllOrdersResponseDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string ServiceTypeAr { get; set; } = null!;
        public string ServiceTypeEn { get; set; } = null!;
        public string StatusAr { get; set; } = null!;
        public string StatusEn { get; set; } = null!;

    }
}
