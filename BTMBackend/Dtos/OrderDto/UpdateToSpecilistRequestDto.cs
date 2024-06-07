namespace BTMBackend.Dtos.OrderDto
{
    public class UpdateToSpecilistRequestDto
    {
        public int OrderId { get; set; }
        public string Note { get; set; } = string.Empty;
        public int? FromEmployeeId { get; set; }
        public int ToEmployeeId { get; set; }
        public DateTime OperationDateTime { get; set; }
    }

}
