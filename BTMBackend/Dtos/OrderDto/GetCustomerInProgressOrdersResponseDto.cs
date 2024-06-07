namespace BTMBackend.Dtos.OrderDto
{
    public class GetCustomerInProgressOrdersResponseDto
    {
        public int Id { get; set; }
        public string ServiceTypeAr { get; set; } = null!;
        public string ServiceTypeEn { get; set; } = null!;
        public string Date { get; set; } = null!;
        public string Time { get; set; } = null!;
        public string StatusAr { get; set; } = null!;
        public string StatusEn { get; set; } = null!;
    }
}
