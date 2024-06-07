using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string TitleAr { get; set; } = null!;
        public string TitleEn { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = null!;

    }
}
