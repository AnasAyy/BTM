namespace BTMBackend.Dtos.PublicDto
{
    public class ListOfOrderIdRequestDto
    {
        public int[] OrderId { get; set; } = null!;
        public string Note { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
    }
}
