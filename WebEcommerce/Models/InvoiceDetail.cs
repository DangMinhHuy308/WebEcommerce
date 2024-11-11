using System.ComponentModel.DataAnnotations;

namespace WebEcommerce.Models
{
    public class InvoiceDetail
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public float? Discount { get; set; }

        // Navigation Properties
        public virtual Invoice Invoice { get; set; }
        public virtual Product Product { get; set; }
    }
}
