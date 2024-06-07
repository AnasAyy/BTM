namespace BTMBackend.Dtos.OrderDto
{
    public class CloseByTechnicianRequestDto
    {
        public int OrderId { get; set; }
        public string OTPCode { get; set; } = null!;
    }
}
