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
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CouponController(ApplicationDbContext context,INotyfService notyfService, IWebHostEnvironment webHostEnvironment)
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
            var coupons = await _context.Coupons.ToListAsync();
            var listOfCouponVM = coupons.Select(x => new CouponVM()
            {
                Id = x.Id,
                Name = x.Name,
                DateStart = x.DateStart,
                DateEnd = x.DateEnd,
                Quantity = x.Quantity,
                Description = x.Description,
                Price = x.Price,
                Status = x.Status
            });
            var pagedCouponVM= listOfCouponVM.ToPagedList(pageNum, pageSize);

            return View(pagedCouponVM);
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CouponVM());
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CouponVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var coupon = new Coupon()
            {
                Name = vm.Name,
                DateStart = vm.DateStart,
                DateEnd = vm.DateEnd.AddDays(7), 
                Description = vm.Description,
                Quantity = vm.Quantity,
                Price = vm.Price,
                Status = vm.Status
            };
            await _context.Coupons.AddAsync(coupon);
            await _context.SaveChangesAsync();
            _notification.Success("Coupon created successfully");
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == id);
            if (coupon == null)
            {
                _notification.Error("coupon not found");
                return View();
            }
            var vm = new CouponVM()
            {
                Id = coupon.Id,
                Name = coupon.Name,
                DateStart = coupon.DateStart,
                DateEnd = coupon.DateEnd.AddDays(7),
                Quantity = coupon.Quantity,
                Price = coupon.Price,
                Status = coupon.Status
            };
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(CouponVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (coupon == null)
            {
                _notification.Error("coupon not found");
                return View(vm);
            }
            coupon.Name = vm.Name;
            coupon.Description = vm.Description;
            coupon.Price = vm.Price;
            coupon.Quantity = vm.Quantity;
            coupon.Description = vm.Description;
            coupon.Status = vm.Status;
            coupon.DateStart = vm.DateStart;
            coupon.DateEnd = vm.DateEnd;

            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
            _notification.Success("Edit successfully");
            return RedirectToAction("Index");

        }

         [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {


            var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == id);
            if (coupon == null)
            {
                _notification.Error("Coupon not found");
                return RedirectToAction("Index");
            }

            // Delete the coupon
            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();

            _notification.Success("Coupon deleted successfully");
            return RedirectToAction("Index");
        }
    }
}
