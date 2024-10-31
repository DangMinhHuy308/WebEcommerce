namespace WebEcommerce.ViewModels
{
    public class CartVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice => Quantity * Price;
    }
}
