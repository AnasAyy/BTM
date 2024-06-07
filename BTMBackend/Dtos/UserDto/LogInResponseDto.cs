namespace BTMBackend.Dtos.UserDto
{
    public class LogInResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PositionAr { get; set; } = null!;
        public string PositionEn { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
