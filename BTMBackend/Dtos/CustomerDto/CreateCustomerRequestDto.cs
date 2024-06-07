using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.CustomerDto
{
    public class CreateCustomerRequestDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int City { get; set; }
        public int County { get; set; }
        public int RoleId { get; set; }
    }
}
