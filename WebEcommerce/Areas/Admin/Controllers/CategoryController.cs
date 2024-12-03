using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using WebEcommerce.Data;
using WebEcommerce.Models;
using WebEcommerce.Utilies;
using WebEcommerce.ViewModels;
using X.PagedList;
using X.PagedList.Extensions;
namespace WebEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        public CategoryController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, INotyfService notyfService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _notification = notyfService;
        }
        // Hiển thị danh sách danh mục
        [Authorize(Roles = "Admin,Author")]
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);

            var categories = await _context.Categories.ToListAsync();

            var listOfCategoriesVM = categories.Select(x => new CategoryVM()
            {
                Id = x.CategoryId,
                Name = x.CategoryName,
                CreatedDate = x.CreatedDate, 
                Image = x.Image
            }).ToList();

            // Phân trang danh sách CategoryVM
            var pagedCategoriesVM = listOfCategoriesVM.ToPagedList(pageNum, pageSize);

            return View(pagedCategoriesVM); 
        }
        // Tạo danh mục
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCategoryVM());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var category = new Category()
            {
                CategoryName = vm.Name,
                Description = vm.Description
            };
            
            
            if (category.CategoryName != null) {
                string slug = vm.Name!.Trim();
                slug = slug.Replace(" ","-");
                category.Slug = slug+ "-"+ Guid.NewGuid();
            }
            if (vm.Thumbnail != null) {
                category.Image = UploadImage(vm.Thumbnail);
            }
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            _notification.Success("Category created successfully");
            return RedirectToAction("Index");
        }
        // Cập nhập danh sách
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id) {
            var category = await _context.Categories.FirstOrDefaultAsync(x=> x.CategoryId == id);
            if(category == null)
            {
                _notification.Error("Category not found");
                return View();
            }
            var vm = new CreateCategoryVM()
            {
                Id = category.CategoryId,
                Name = category.CategoryName,
                Description = category.Description,
                Image = category.Image,
            };
            return View(vm);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(CreateCategoryVM vm)
        {
            if (!ModelState.IsValid) { 
                return View(vm);
            }
            var category = await _context.Categories.FirstOrDefaultAsync(x=>x.CategoryId == vm.Id);
            if (category == null) {
                _notification.Error("Category not found");
                return View(vm);
            }
            category.CategoryName = vm.Name;
            category.Description = vm.Description;
            if (vm.Thumbnail!= null) {
                category.Image = UploadImage(vm.Thumbnail);
            }

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            _notification.Success("Edit successfully");
            return RedirectToAction("Index");

        }
        //Xóa danh mục
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Find the category by ID
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (category == null)
            {
                _notification.Error("Category not found.");
                return RedirectToAction("Index", "Category", new { area = "Admin" });
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            _notification.Success("Delete successfully");
            return RedirectToAction("Index", "Category", new { area = "Admin" });
        }
        // Hiẻn thị hình ảnh
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
