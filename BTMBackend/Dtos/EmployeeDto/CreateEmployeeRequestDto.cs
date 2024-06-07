using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.EmployeeDto
{
    public class CreateEmployeeRequestDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int City { get; set; }
        public int County { get; set; }
        public string Address { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
    }
}
