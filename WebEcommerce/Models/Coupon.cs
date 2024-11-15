using System.ComponentModel.DataAnnotations;

namespace WebEcommerce.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime DateStart { get; set; } = DateTime.Now;
        public DateTime DateEnd { get; set; }
        public int Status { get; set; }
        public int Quantity{ get; set; }
        public decimal? Price { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}
