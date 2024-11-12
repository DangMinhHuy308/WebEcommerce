namespace WebEcommerce.ViewModels
{
    public class CategoryWithProductVM
    {
        public string CategoryName { get; set; }
        public IEnumerable<ProductVM> Products { get; set; }
    }
}
