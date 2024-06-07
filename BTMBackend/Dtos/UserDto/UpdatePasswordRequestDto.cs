namespace BTMBackend.Dtos.UserDto
{
    public class UpdatePasswordRequestDto
    {
        public int Id { get; set; }
        public string Password { get; set; } = null!;
    }
}
