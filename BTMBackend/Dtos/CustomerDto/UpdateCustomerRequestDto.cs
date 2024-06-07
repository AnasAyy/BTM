namespace BTMBackend.Dtos.CustomerDto
{
    public class UpdateCustomerRequestDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = null!;
        public int City { get; set; }
        public int County { get; set; }
    }
}
