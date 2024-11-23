using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Data;
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

            // lấy toàn bộ mã giảm giá trong cơ sở dữ liệu
            var shippings = await _context.Shippings.ToListAsync();
            var listOfShippingVM = shippings.Select(x => new ShippingVM()
            {
                Id = x.Id,
                Ward = x.Ward,
                District = x.District,
                City = x.City,
                Price = x.Price,
            });

            var pagedCouponVM = listOfShippingVM.ToPagedList(pageNum, pageSize);
            return View(pagedCouponVM);
        }
    }
}
