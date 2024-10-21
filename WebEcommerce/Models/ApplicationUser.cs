using Microsoft.AspNetCore.Identity;

namespace WebEcommerce.Models
{
	public class ApplicationUser: IdentityUser
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public bool? Gender { get; set; } // Nullable để có thể không có thông tin
		public DateTime? DateOfBirth { get; set; } // Nullable để có thể không có thông tin
		public string Image { get; set; }
		public bool IsActive { get; set; }
		// Navigation Properties
		public virtual ICollection<Invoice> Invoices { get; set; }
	}
}
