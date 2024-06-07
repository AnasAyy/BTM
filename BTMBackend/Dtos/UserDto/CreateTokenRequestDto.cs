namespace BTMBackend.Dtos.UserDto
{
    public class CreateTokenRequestDto
    {
        public int UserId { get; set; }
        public string Role { get; set; } = null!;
    }
}
