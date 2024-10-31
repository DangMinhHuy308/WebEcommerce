using Microsoft.AspNetCore.Identity;

namespace WebEcommerce.Models
{
	public class ApplicationUser: IdentityUser
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
        public string? Image { get; set; }
        public string? Address { get; set; }
		public bool Gender{ get; set; } = true;


        // Navigation Properties
        public virtual ICollection<Invoice> Invoices { get; set; }
	}
}
