namespace WebEcommerce.ViewModels
{
    public class ProfileVM
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Image{ get; set; }
        public bool Gender { get; set; }
        public IFormFile? Thumbnail { get; set; } 
    }
}
