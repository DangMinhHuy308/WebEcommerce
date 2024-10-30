using Microsoft.AspNetCore.Mvc;
using WebEcommerce.Data;
using WebEcommerce.ViewModels;

namespace WebEcommerce.Controllers
{
    public class ProductController : Controller
    {
        public readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? id)
        {
            var product = _context.Products.AsQueryable();
            if (id.HasValue) { 
                product = product.Where(x => x.CategoryId == id.Value);
            }
            var result = product.Select(x => new ProductVM
            {
                Id = x.ProductId,
                Name = x.ProductName,
                Price = x.Price,
                Description = x.Description,
                Image = x.Image
            });
            return View(result);
        }
    }
}
