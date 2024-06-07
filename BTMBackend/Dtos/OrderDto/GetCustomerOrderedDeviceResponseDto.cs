namespace BTMBackend.Dtos.OrderDto
{
    public class GetCustomerOrderedDeviceResponseDto
    {
        public int Id { get; set; }
        public string DeviceNameAr { get; set; } = null!;
        public string DeviceNameEn { get; set; } = null!;
        public string WarrantyExpirationDate { get; set; } = null!;
        public string WarrantyStatusAr { get; set; } = null!;
        public string WarrantyStatusEn { get; set; } = null!;

    }
}
