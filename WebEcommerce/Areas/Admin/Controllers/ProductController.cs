using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Data;
using WebEcommerce.Migrations;
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
                Price = p.Price,
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


            var vm = new CreateProductVM
            {
                Categories = await _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToListAsync(),

                Suppliers = await _context.Suppliers.Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.CompanyName
                }).ToListAsync()
            };

            // Đảm bảo rằng cả hai danh sách đều không null
            if (vm.Categories == null)
            {
                vm.Categories = new List<SelectListItem>();
            }

            if (vm.Suppliers == null)
            {
                vm.Suppliers = new List<SelectListItem>();
            }

            return View(vm);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM vm)
        {
            vm.Categories = await _context.Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToListAsync();

            vm.Suppliers = await _context.Suppliers.Select(s => new SelectListItem
            {
                Value = s.SupplierId.ToString(),
                Text = s.CompanyName
            }).ToListAsync();

            if (ModelState.IsValid)
            {
                // Tạo một đối tượng Product mới từ ViewModel
                var product = new Product
                {
                    ProductName = vm.ProductName,
                    Alias = vm.Alias,
                    CategoryId = vm.CategoryId,
                    Price = vm.Price,
                    Sale = vm.Sale,
                    OriginalPrice = vm.OriginalPrice,
                    UnitDescription = vm.UnitDescription,
                    Description = vm.Description,
                    SupplierId = vm.SupplierId,
                    IsActive = vm.IsActive,
                    IsSale = vm.IsSale,
                };
                if (product.ProductName != null)
                {
                    string slug = vm.ProductName!.Trim();
                    slug = slug.Replace(" ", "-");
                    product.Slug = slug + "-" + Guid.NewGuid();
                }
                if (vm.Thumbnail != null)
                {
                    product.Image = UploadImage(vm.Thumbnail);
                }
                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                _notification.Success("Add successfully");
                return RedirectToAction("Index");
            }

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var vm = new CreateProductVM
            {
                ProductName = product.ProductName,
                Alias = product.Alias,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId,
                Image = product.Image,
                Price = product.Price,
                Sale = product.Sale,
                OriginalPrice = product.OriginalPrice,
                UnitDescription = product.UnitDescription,
                Description = product.Description,
                IsActive = product.IsActive,
                IsSale = product.IsSale,
                Categories = await _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToListAsync(),
                Suppliers = await _context.Suppliers.Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.CompanyName
                }).ToListAsync()
            };

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit( CreateProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                // Lấy lại danh sách Category và Supplier nếu ModelState không hợp lệ
                vm.Categories = await _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToListAsync();
                vm.Suppliers = await _context.Suppliers.Select(s => new SelectListItem
                {
                    Value = s.SupplierId.ToString(),
                    Text = s.CompanyName
                }).ToListAsync();

                return View(vm);
            }

            var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == vm.Id);
            if (product == null)
            {
                _notification.Error("Product not found");
                return View(vm);
            }

            // Cập nhật thông tin sản phẩm
            product.ProductName = vm.ProductName;
            product.Alias = vm.Alias;
            product.CategoryId = vm.CategoryId;
            product.Price = vm.Price;
            product.Sale = vm.Sale;
            product.OriginalPrice = vm.OriginalPrice;
            product.UnitDescription = vm.UnitDescription;
            product.Description = vm.Description;
            product.SupplierId = vm.SupplierId;
            product.IsActive = vm.IsActive;
            product.IsSale = vm.IsSale;

            if (vm.Thumbnail != null)
            {
                product.Image = UploadImage(vm.Thumbnail);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            _notification.Success("Edit successfully");
            // Chuyển hướng sau khi cập nhật thành công
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
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
