using System.ComponentModel.DataAnnotations;

namespace BTMBackend.Dtos.Product
{
    public class AddAccessoriesAndFeatures
    {
        [Required]
        public int ProductId { get; set; }

        public List<AccessoriesFeatures> Accessories { get; set; } = null!;

        public List<AccessoriesFeatures> Features { get; set; } = null!;
    }

    public class AccessoriesFeatures
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
    }
}
