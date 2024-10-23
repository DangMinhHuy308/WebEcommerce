namespace WebEcommerce.Models
{
	public class Category
	{
		public int CategoryId { get; set; }

		public string? CategoryName { get; set; } = null!;

		public string? Alias { get; set; }

		public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public virtual ICollection<Product> Products { get; set; }
	}
}
