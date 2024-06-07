using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.AuthorityDto
{
    public class CreateRoleRequestDto
    {
        public string TitleAr { get; set; } = null!;
        public string TitleEn { get; set; } = null!;
    }
}
