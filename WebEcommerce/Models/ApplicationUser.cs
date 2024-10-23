using Microsoft.AspNetCore.Identity;

namespace WebEcommerce.Models
{
	public class ApplicationUser: IdentityUser
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		
		// Navigation Properties
		public virtual ICollection<Invoice> Invoices { get; set; }
	}
}
