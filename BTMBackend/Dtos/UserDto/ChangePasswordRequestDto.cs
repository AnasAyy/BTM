namespace BTMBackend.Dtos.UserDto
{
    public class ChangePasswordRequestDto
    {
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
