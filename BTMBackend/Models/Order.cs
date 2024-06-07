using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTMBackend.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ServiceType { get; set; }
        [Required]
        public int CountyId { get; set; }
        [Required]
        public int CityId { get; set; }
        [Required]
        public string Address { get; set; } = null!;
        [Required]
        public string Message { get; set; } = string.Empty;
        public int Status { get; set; }
        // 0-UnderReviewFromSuperviser 1-SentToCallCenter 2-PrepaireToCancelFromCallCenter
        // 3-SentToTechnician 4-PrepaireToCancelFromTechnician 5-Canceled 6-FilesSentBySpecilist
        // 7-PrepaireToDone 8-Done
        public string Note { get; set; } = string.Empty;
        public DateTime? OperationDateTime { get; set; }
        public DateTime? InstallationDate {  get; set; }
        public string AttachmentPath { get; set; } = string.Empty; //Pdf file after the order is finished completily
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("CustomerId")]
        public int? CustomerId { get; set; }
        [ForeignKey("EmployeeId")]
        public int? EmployeeId { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = null!;
        public ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;
        public ICollection<CustomerProduct> CustomerProducts { get; set; } = null!;
        public ICollection<CustomerProductPart> CustomerProductParts { get; set; } = null!;

    }
}
