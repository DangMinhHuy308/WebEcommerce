namespace WebEcommerce.Models
{
	public class Invoice
	{
		public int InvoiceId { get; set; }
		public string? ApplicationUserId { get; set; }

		public DateTime? OrderDate { get; set; } = DateTime.Now;
		public DateTime? RequiredDate { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public string? Address { get; set; }
		public string? PaymentMethod { get; set; }
		public string? ShippingMethod { get; set; }
		public float? ShippingFee { get; set; }
		public int StatusId { get; set; }
		public string? Notes { get; set; }
		
		public virtual ApplicationUser ApplicationUser { get; set; }
		public virtual ICollection<InvoiceDetail> InvoiceDetails{ get; set; }
	}
}
