namespace WebEcommerce.Models
{
	public class Supplier
	{
		public int SupplierId { get; set; }
		public string CompanyName { get; set; }
		public string Logo { get; set; }
		public string ContactPerson { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public string Description { get; set; }

		// Navigation Properties
		public virtual ICollection<Product> Products { get; set; }
	}
}
