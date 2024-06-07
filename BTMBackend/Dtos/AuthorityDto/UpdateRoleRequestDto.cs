namespace BTMBackend.Dtos.AuthorityDto
{
    public class UpdateRoleRequestDto
    {
        public int Id { get; set; }
        public string TitleAr { get; set; } = null!;
        public string TitleEn { get; set; } = null!;
    }
}
