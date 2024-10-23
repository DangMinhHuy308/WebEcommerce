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
        [Authorize(Roles = "Admin,Author")]

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);

            // Lấy danh sách danh mục từ cơ sở dữ liệu một cách không đồng bộ
            var categories = await _context.Categories.ToListAsync(); // Sử dụng ToListAsync để lấy dữ liệu không đồng bộ

            // Chuyển đổi danh sách danh mục thành danh sách CategoryVM
            var listOfCategoriesVM = categories.Select(x => new CategoryVM()
            {
                Id = x.CategoryId,
                Name = x.CategoryName,
                CreatedDate = x.CreatedDate, // Sử dụng CreatedDate từ danh mục
                Image = x.Image
            }).ToList();

            // Phân trang danh sách CategoryVM
            var pagedCategoriesVM = listOfCategoriesVM.ToPagedList(pageNum, pageSize);

            return View(pagedCategoriesVM); // Trả về danh sách phân trang của CategoryVM
        }
    }
}
