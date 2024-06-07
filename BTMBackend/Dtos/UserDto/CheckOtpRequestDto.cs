namespace BTMBackend.Dtos.UserDto
{
    public class CheckOtpRequestDto
    {
        public string PhoneNumber { get; set; } = null!;
        public string OTPCode { get; set; } = null!;
    }
}
