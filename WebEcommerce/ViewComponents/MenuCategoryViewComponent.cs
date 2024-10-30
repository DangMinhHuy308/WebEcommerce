using Microsoft.AspNetCore.Mvc;
using WebEcommerce.Data;
using WebEcommerce.ViewModels;

namespace WebEcommerce.ViewComponents
{
    public class MenuCategoryViewComponent: ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public MenuCategoryViewComponent(ApplicationDbContext context) => _context = context;
        public IViewComponentResult Invoke()
        {
            var data = _context.Categories.Select(x => new MenuCategoryVM
            {
                Id = x.CategoryId,
                Name = x.CategoryName,
            }).OrderBy(x => x.Name);
            return View("Index",data);
        }
    }
}
