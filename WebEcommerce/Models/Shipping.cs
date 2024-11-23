namespace WebEcommerce.Models
{
    public class Shipping
    {
        public int Id { get; set; }
        public string? Ward{ get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public decimal? Price { get; set; }
    }
}
