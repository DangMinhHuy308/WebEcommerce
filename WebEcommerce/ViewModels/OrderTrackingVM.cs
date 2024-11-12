namespace WebEcommerce.ViewModels
{
    public class OrderTrackingVM
    {
        public int InvoiceId { get; set; }
        public string Code { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int StatusId { get; set; }
        public List<OrderItemVM> Items { get; set; }
    }
    public class OrderItemVM
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public float? Discount { get; set; }
        public decimal Total => Quantity * Price * (1 - (decimal)(Discount ?? 0));
    }
}
