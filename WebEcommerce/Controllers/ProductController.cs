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
        // Hiển thị danh sách sản phẩm theo danh mục và phân trang
        public IActionResult Index(int? id, int? page)
        {
            int pageSize = 5;
            int pageNum = page ?? 1;
            // Lấy tất cả sản sẩm trong cơ sở dữ liệu
            var product = _context.Products.AsQueryable();
            // Nếu có id danh mục, lọc sản phẩm theo danh mục đó
            if (id.HasValue) { 
                product = product.Where(x => x.CategoryId == id.Value);
            }
            // Chuyển dữ liệu sản phẩm thành dạng ViewModel
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
        // Tìm kiếm sản phẩm theo tên
        public IActionResult Search(string? query)
        {
            // Lấy tất cả sản sẩm trong cơ sở dữ liệu
            var product = _context.Products.AsQueryable();
            // Nếu có truy vấn tìm kiếm, lọc sản phẩm theo tên chứa chuỗi tìm kiếm
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
        // Hiển thị chi tiết sản phẩm
        public IActionResult Detail(int id) 
        {
            // Lấy thông tin chi tiết sản phẩm bao gồm cả danh mục
            var product = _context.Products
            .Include(x => x.Category) 
            .SingleOrDefault(x => x.ProductId == id);

            if (product == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }
            // Lấy tất cả sản phẩm liên quan trong cùng 1 danh mục
			var relatedProducts = _context.Products
		    .Where(x => x.CategoryId == product.CategoryId && x.ProductId != product.ProductId && x.IsRelated)
		    .Take(5) // Giới hạn số lượng sản phẩm liên quan
		    .ToList();
            // Chuyển dữ liệu sản phẩm chi tiết và sản phẩm liên quan thành ViewModel
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
