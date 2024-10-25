using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Data;
using WebEcommerce.Models;
using WebEcommerce.ViewModels;
using X.PagedList.Extensions;

namespace WebEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, INotyfService notyfService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _notification = notyfService;
        }
        [Authorize(Roles = "Admin,Author")]

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNum = page ?? 1;

            // Lấy danh sách sản phẩm cùng với danh mục
            var products = await _context.Products.Include(p => p.Category).ToListAsync();

            var listOfProductVM = products.Select(p => new ProductVM()
            {
                Id = p.ProductId,
                Name = p.ProductName,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName, // Thêm kiểm tra null
                CreatedDate = p.CreatedDate,
                Description = p.Description,
                Image = p.Image
            }).ToList();

            // Phân trang danh sách sản phẩm
            var pagedProductVM = listOfProductVM.ToPagedList(pageNum, pageSize);
            return View(pagedProductVM);
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories
                         .Select(c => new SelectListItem
                         {
                             Value = c.CategoryId.ToString(),
                             Text = c.CategoryName
                         })
                         .ToListAsync();

            var vm = new CreateProductVM
            {
                Categories = categories
            };

            return View(vm);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                // Cần truyền lại danh sách thể loại nếu ModelState không hợp lệ
                vm.Categories = await _context.Categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    })
                    .ToListAsync();
                return View(vm);
            }

            var product = new Product()
            {
                ProductName = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                OriginalPrice = vm.OriginalPrice,
                Sale = vm.Sale,
                CategoryId = vm.CategoryId // Lưu ID thể loại vào sản phẩm
            };

            // Tạo slug cho sản phẩm
            if (product.ProductName != null)
            {
                string slug = vm.Name!.Trim();
                slug = slug.Replace(" ", "-");
                product.Slug = slug + "-" + Guid.NewGuid();
            }

            if (vm.Thumbnail != null)
            {
                product.Image = UploadImage(vm.Thumbnail);
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            _notification.Success("Product created successfully");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Find the category by ID
            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            if (product == null)
            {
                _notification.Error("Product not found.");
                return RedirectToAction("Index", "Product", new { area = "Admin" });
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _notification.Success("Delete successfully");
            return RedirectToAction("Index", "Product", new { area = "Admin" });
        }
        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;

        }
    }
}
