using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Models
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }
        public int UserId { get; set; }
    }
}
