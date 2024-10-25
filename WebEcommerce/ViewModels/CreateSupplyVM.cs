namespace WebEcommerce.ViewModels
{
    public class CreateSupplyVM
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? ContactPerson { get; set; }

        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

        public string? Description { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public string? Image { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
