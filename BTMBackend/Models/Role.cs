using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string TitleAr { get; set; } = null!;
        [Required]
        public string TitleEn { get; set; } = null!;
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<User> Users { get; set; } = null!;
        public ICollection<RolePermission> RolePermissions { get; set; } = null!;
    }
}
