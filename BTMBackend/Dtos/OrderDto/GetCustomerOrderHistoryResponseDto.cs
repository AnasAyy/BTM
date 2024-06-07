namespace BTMBackend.Dtos.OrderDto
{
    public class GetCustomerOrderHistoryResponseDto
    {
        public string ServiceTypeAr { get; set; } = null!;
        public string ServiceTypeEn { get; set; } = null!;
        public string StatusAr { get; set; } = null!;
        public string StatusEn { get; set; } = null!;
        public string OrderClosingDAte { get; set; } = null!;
    }
}
