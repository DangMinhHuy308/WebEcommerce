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

        public List<Post>? Posts { get; set; }
        // Navigation Properties
        public virtual ICollection<Invoice> Invoices { get; set; }
        // Các tin nhắn người dùng gửi
        public virtual ICollection<Message> SentMessages { get; set; }

        // Các tin nhắn người dùng nhận
        public virtual ICollection<Message> ReceivedMessages { get; set; }
    }
}
