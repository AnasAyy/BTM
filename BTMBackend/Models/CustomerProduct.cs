using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class CustomerProduct
    {
        [Key]
        public int Id { get; set; }
        public DateTime ExpirationDate { get; set; }

        [ForeignKey(nameof(Product.Id))]
        public int? ProductId { get; set; }

        [ForeignKey(nameof(Customer.Id))]
        public int? CustomerId { get; set; }

        [ForeignKey(nameof(Order.Id))]
        public int? OrderId { get; set;}

        public ICollection<CustomerProductPart> CustomerProductParts { get; set; } = null!;

    }
}
