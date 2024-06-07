namespace BTMBackend.Dtos.OrderDto
{
    public class GetCustomerPartDetailsResponseDto
    {
        public int Id { get; set; }
        public string PartNameAr { get; set; } = null!;
        public string PartNameEn { get; set; } = null!;
        public string MaintenanceExpirationDate { get; set; } = null!;
    }
}
