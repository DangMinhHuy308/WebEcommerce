namespace WebEcommerce.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
