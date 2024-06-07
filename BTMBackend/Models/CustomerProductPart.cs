using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class CustomerProductPart
    {
        [Key]
        public int Id { get; set; }

        public DateTime MaintenanceDate { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        [ForeignKey(nameof(Part.Id))]
        public int? PartId { get; set; }

        [ForeignKey(nameof(CustomerProduct.Id))]
        public int? CustomerProductId { get; set;}

        [ForeignKey(nameof(Order.Id))]
        public int? OrderId { get; set; }

    }
}
