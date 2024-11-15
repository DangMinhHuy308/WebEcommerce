namespace WebEcommerce.ViewModels
{
    public class CouponVM
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateStart { get; set; } = DateTime.Now; 
        public DateTime DateEnd { get; set; } = DateTime.Now.AddDays(7); 
        public int Status { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }

    }
}
