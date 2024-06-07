namespace BTMBackend.Dtos.UserDto
{
    public class ChangeUserPasswordRequestDto
    {
        public int UserId { get; set; }
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
