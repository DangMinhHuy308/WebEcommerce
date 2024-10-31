using System.ComponentModel.DataAnnotations;

namespace WebEcommerce.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]

        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]

        public string? UserName { get; set; }
        [Required]

        public string? Password { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Address { get; set; }
        public bool Gender { get; set; } = true;
        public string? Image { get; set; }
		public IFormFile? Thumbnail { get; set; }
		public bool IsAdmin { get; set; }
    }
}
