	namespace WebEcommerce.Models
{
	public class Status
	{
		public int StatusId { get; set; }
		public string StatusName { get; set; }
		public string Description { get; set; }

		// Navigation Properties
		public virtual ICollection<Invoice> Invoices { get; set; }
	}
}
