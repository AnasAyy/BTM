namespace BTMBackend.Models
{
    public class OTPMessage
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int Status { get; set; } // 0-pending 1-Verified 2-Cancelled
        public DateTime ExpirationDatetime { get; set; }
    }
}
