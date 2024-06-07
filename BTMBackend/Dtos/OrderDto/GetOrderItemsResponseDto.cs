namespace BTMBackend.Dtos.OrderDto
{
    public class GetOrderItemsResponseDto
    {
        public int Id { get; set; }
        public byte[]? Image { get; set; }
        public string ItemNameAr { get; set; } = null!;
        public string ItemTypeAr { get; set; } = null!;
        public string ItemNameEn { get; set; } = null!;
        public string ItemTypeEn { get; set; } = null!;
    }
}
