namespace BTMBackend.Dtos.EmployeeDto
{
    public class GetAllEmployeeResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PositionAr { get; set; } = null!;
        public string PositionEn { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
