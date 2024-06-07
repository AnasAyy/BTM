using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("RoleId")]
        public int? RoleId { get; set; }

        [ForeignKey("PermissionId")]
        public int? PermissionId { get; set; }
    }
}
