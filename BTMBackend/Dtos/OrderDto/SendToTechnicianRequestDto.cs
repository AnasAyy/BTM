namespace BTMBackend.Dtos.OrderDto
{
    public class SendToTechnicianRequestDto
    {
        public int OrderId { get; set; }
        public string Note { get; set; } = string.Empty;
        public int ToEmployeeId { get; set; }
        public DateTime OperationDateTime { get; set; }
    }
}
