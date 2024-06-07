namespace BTMBackend.Dtos.EmployeeDto
{
    public class UpdateEmployeeByIdRequestDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int City { get; set; }
        public int County { get; set; }
        public string Address { get; set; } = null!;
        public bool IsAcyive { get; set; }
        public int RoleId { get; set; }
    }
}
