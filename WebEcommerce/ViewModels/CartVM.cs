namespace WebEcommerce.ViewModels
{
    public class CartVM
    {
        public int ProductId { get; set; }

        public string Name { get; set; }
        public string? Image { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice => Quantity * Price;
    }
    public class CartIconVM
    {
        public decimal? Quantity { get; set; }
        public decimal? Total { get; set; }
    }
}
