using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.EmployeeDto
{
    public class GetEmployeeResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string CityAr { get; set; } = null!;
        public string CityEn { get; set; } = null!;
        public string CountyAr { get; set; } = null!;
        public string CountyEn { get; set; } = null!;
        public string Address { get; set; } = string.Empty;
        public string RoleAr { get; set; } = null!;
        public string RoleEn { get; set; } = null!;
    }
}
