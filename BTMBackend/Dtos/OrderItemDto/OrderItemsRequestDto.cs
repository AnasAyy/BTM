namespace BTMBackend.Dtos.OrderItemDto
{
    public class OrderItemsRequestDto
    {
        public int OrderId { get; set; }
        public int[] ItemType { get; set; } = null!;  // 1-Product 2-Part
        public int[] ServiceType { get; set; } = null!; // 1-New // 2-Maintenance
        public int[] ItemId { get; set; } = null!;
        public int[] CustomerPartId { get; set; } = null!;
    }
}
