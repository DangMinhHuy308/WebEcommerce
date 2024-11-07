namespace WebEcommerce.Models
{
	public class Product
	{
		public int ProductId { get; set; }
		public string? ProductName { get; set; }
		public string? Alias { get; set; }
		public int CategoryId { get; set; }
		public string? Image { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public decimal? Price { get; set; }
        public decimal? Sale { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int? ViewCount { get; set; }
		public string? UnitDescription { get; set; }
		public string? Description { get; set; }
        public int? Quantity { get; set; }
        public int SupplierId { get; set; }
        public string? Slug { get; set; }
        public bool IsActive { get; set; } 
        public bool IsSale { get; set; } 
        public bool IsRelated { get; set; }

        // Navigation Properties
        public virtual Category Category { get; set; }
		public virtual Supplier Supplier { get; set; }
		public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }

    }
}
