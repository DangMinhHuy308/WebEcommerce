namespace WebEcommerce.ViewModels
{
    public class ProductDetailVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public string? Detail { get; set; }
        public int? Review { get; set; }
        public int? Count { get; set; }
    }
}
