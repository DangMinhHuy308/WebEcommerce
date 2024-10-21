namespace WebEcommerce.Models
{
	public class Invoice
	{
		public int InvoiceID { get; set; }
		public string CustomerID { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime? RequiredDate { get; set; }
		public DateTime? DeliveryDate { get; set; }
		public string FullName { get; set; }
		public string Address { get; set; }
		public string PaymentMethod { get; set; }
		public string ShippingMethod { get; set; }
		public float ShippingFee { get; set; }
		public int StatusID { get; set; }
		public string EmployeeID { get; set; }
		public string Notes { get; set; }
		
		public virtual ApplicationUser ApplicationUser { get; set; }
		//public virtual TrangThai TrangThai { get; set; }
		public virtual ICollection<InvoiceDetail> InvoiceDetails{ get; set; }
	}
}
