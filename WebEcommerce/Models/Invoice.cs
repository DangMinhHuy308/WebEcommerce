namespace WebEcommerce.Models
{
	public class Invoice
	{
		public int InvoiceId { get; set; }
		public string ApplicationUserId { get; set; }

		public DateTime OrderDate { get; set; }
		public DateTime? RequiredDate { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public string FullName { get; set; }
		public string Address { get; set; }
		public string PaymentMethod { get; set; }
		public string ShippingMethod { get; set; }
		public float ShippingFee { get; set; }
		public int StatusId { get; set; }
		public string Notes { get; set; }
		
		public virtual ApplicationUser ApplicationUser { get; set; }
		public virtual Status Status { get; set; }
		public virtual ICollection<InvoiceDetail> InvoiceDetails{ get; set; }
	}
}
