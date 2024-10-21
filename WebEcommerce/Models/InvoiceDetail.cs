namespace WebEcommerce.Models
{
	public class InvoiceDetail
	{
		public int InvoiceDetailID { get; set; }
		public int InvoiceID { get; set; }
		public int ProductID { get; set; }
		public float UnitPrice { get; set; }
		public int Quantity { get; set; }
		public float Discount { get; set; }

		// Navigation Properties
		public virtual Invoice Invoice { get; set; }
		public virtual Product Product { get; set; }
	}
}
