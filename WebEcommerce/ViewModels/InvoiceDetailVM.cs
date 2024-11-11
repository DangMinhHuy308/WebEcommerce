namespace WebEcommerce.ViewModels
{
    public class InvoiceDetailVM
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public string? Code { get; set; }
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }

        public int? Quantity { get; set; }
    }
}
