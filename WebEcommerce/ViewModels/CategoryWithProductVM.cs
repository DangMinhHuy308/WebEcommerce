namespace WebEcommerce.ViewModels
{
    public class CategoryWithProductVM
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }
        public IEnumerable<ProductVM> Products { get; set; }
        public IEnumerable<MenuCategoryVM> MenuCategories { get; set; }

    }
}
