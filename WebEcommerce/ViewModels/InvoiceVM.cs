namespace WebEcommerce.ViewModels
{
    public class InvoiceVM
    {
        public int Id { get; set; }
        public int? InvoiceDetailId { get; set; }
        public string? Code { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.Now;
        public DateTime? RequiredDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ShippingMethod { get; set; }
        public decimal? ShippingFee { get; set; }
        public int StatusId { get; set; }
        public string? Notes { get; set; }
        //public int? Quantity { get; set; }

        //public decimal? Price { get; set; }
        //public decimal? TotalPrice => Quantity * Price;
    }
}
