using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Data;
using WebEcommerce.Models;
using WebEcommerce.ViewModels;
using X.PagedList.Extensions;

namespace WebEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ShippingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ShippingController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _notification = notyfService;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize(Roles = "Admin,Author")]
        [HttpGet]
        
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNum = page ?? 1;

            var shippings = await _context.Shippings.ToListAsync();
            var listOfShippingVM = shippings.Select(x => new ShippingVM()
            {
                Id = x.Id,
                Ward = x.Ward,
                District = x.District,
                City = x.City,
                Price = x.Price,
            });

            var pagedShippingVM = listOfShippingVM.ToPagedList(pageNum, pageSize);
            return View(pagedShippingVM);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Shipping shippingModel, string phuong, string quan, string tinh, decimal price)
        {
            shippingModel.City= tinh;
            shippingModel.District = quan;
            shippingModel.Ward = phuong;
            shippingModel.Price = price;
            try
            {
                var shipping= await _context.Shippings
                    .AnyAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);
                if (shipping)
                {
                    return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp" });
                }
                _context.Shippings.Add(shippingModel);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Thành công" });
            }
            catch(Exception)
            {
                return StatusCode(500,"An error occurred while processing your request.");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm mã giảm giá theo id
            var shipping = await _context.Shippings.FirstOrDefaultAsync(x => x.Id == id);
            if (shipping == null)
            {
                _notification.Error("Shippings not found");
                return RedirectToAction("Index");
            }

            // Xóa mã giảm giá khỏi csdl
            _context.Shippings.Remove(shipping);
            await _context.SaveChangesAsync();
            _notification.Success("Shippings deleted successfully");
            return RedirectToAction("Index");
        }
    }
}
