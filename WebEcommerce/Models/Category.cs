namespace WebEcommerce.Models
{
	public class Category
	{
		public int CategoryId { get; set; }

		public string CategoryName { get; set; } = null!;

		public string? Alias { get; set; }

		public string? Description { get; set; }

		public string? Image { get; set; }
		public virtual ICollection<Product> Products { get; set; }
	}
}
