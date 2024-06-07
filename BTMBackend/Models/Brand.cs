namespace BTMBackend.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string Logo { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public int UserId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

    }
}
