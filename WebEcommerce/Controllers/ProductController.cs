using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Data;
using WebEcommerce.ViewModels;
using X.PagedList.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebEcommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? id, int? page)
        {
            int pageSize = 5;
            int pageNum = page ?? 1;
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
            // Thực hiện phân trang
            var pagedProductVM = result.ToPagedList(pageNum, pageSize);

            return View(pagedProductVM);
        }
        public IActionResult Search(string? query)
        {
            var product = _context.Products.AsQueryable();
            if (query != null)
            {
                product = product.Where(x => x.ProductName.Contains(query));
            }
            var result = product.Select(x => new ProductVM
            {
                Id = x.ProductId,
                Name = x.ProductName,
                Price = x.Price,
                Image = x.Image
            });
            return View(result);
        }
        public IActionResult Detail(int id) 
        {
            var product = _context.Products
            .Include(x => x.Category) 
            .SingleOrDefault(x => x.ProductId == id);

            if (product == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }
			var relatedProducts = _context.Products
		    .Where(x => x.CategoryId == product.CategoryId && x.ProductId != product.ProductId && x.IsRelated)
		    .Take(5) // Giới hạn số lượng sản phẩm liên quan
		    .ToList();
			var result = new ProductDetailVM
            {
                Id = product.ProductId,
                Name = product.ProductName,
                Price = product.Price,
                Description = product.Description,
                Image = product.Image,
                CategoryName = product.Category?.CategoryName,

				RelatedProducts = relatedProducts
			};

            return View(result); 
        }
    }
}
