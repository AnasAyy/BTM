namespace BTMBackend.Dtos.OrderDto
{
    public class OrderContactDetails
    {
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string InstallationDate { get; set; } = null!;
        public string InstallationTime { get; set; } = null!;
        public string AddressAr { get; set; } = null!;
        public string AddressEn { get; set; } = null!;
        public string AddressDetails { get; set; } = null!;
    }

    public class GetOrderDetailsResponseDto
    {
        public OrderContactDetails OrderContactDetails { get; set; } = null!;

        public List<OrderDetails> OrderDetails { get; set; } = null!;
    }

    public class OrderDetails
    {
        public string ItemNameAr { get; set; } = null!;
        public string ItemNameEn { get; set; } = null!;
        public byte[]? ItemPhoto { get; set; } = null!;
        public string ItemTypeAr { get; set; } = null!;
        public string ItemTypeEn { get; set; } = null!;
        public string ServiceTypeAr { get; set; } = null!;
        public string ServiceTypeEn { get; set; } = null!;
    }
}
