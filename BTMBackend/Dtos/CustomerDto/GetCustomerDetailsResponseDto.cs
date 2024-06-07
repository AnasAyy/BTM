namespace BTMBackend.Dtos.CustomerDto
{
    public class GetCustomerDetailsResponseDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int County { get; set; }
        public int City { get; set; }
        public string? Address { get; set; }
    }
}
