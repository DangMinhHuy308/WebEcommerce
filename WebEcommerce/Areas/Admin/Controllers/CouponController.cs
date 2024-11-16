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

        // Hiển thị danh sách mã giảm giá
        [Authorize(Roles = "Admin,Author")]
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNum = page ?? 1;

            // lấy toàn bộ mã giảm giá trong cơ sở dữ liệu
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

        // Thêm mã giảm giá mới (hiển thị form)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CouponVM());
        }

        // Xử lý tạo mã giảm giá (lưu vào csdl)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CouponVM vm)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Tạo một đối tượng Coupon mới từ dữ liệu
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

            // Lưu coupon vào csdl
            await _context.Coupons.AddAsync(coupon);
            await _context.SaveChangesAsync();
            _notification.Success("Coupon created successfully");
            return RedirectToAction("Index");
        }

        // Cập nhập mã giảm giá (hiển thị form)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm mã giảm giá theo id đã lưu trong csdl
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == id);
            if (coupon == null)
            {
                _notification.Error("coupon not found");
                return View();
            }

            // Hiển thị view với các thuộc tính bên dưới
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

        // Xử lý cập nhật mã giảm giá
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(CouponVM vm)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Tìm mã giảm giá theo ID 
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (coupon == null)
            {
                _notification.Error("coupon not found");
                return View(vm);
            }

            // Cập nhật các thuộc tính của coupon
            coupon.Name = vm.Name;
            coupon.Description = vm.Description;
            coupon.Price = vm.Price;
            coupon.Quantity = vm.Quantity;
            coupon.Description = vm.Description;
            coupon.Status = vm.Status;
            coupon.DateStart = vm.DateStart;
            coupon.DateEnd = vm.DateEnd;

            //  Cập nhật coupon trong cơ sở dữ liệu
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
            _notification.Success("Edit successfully");
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm mã giảm giá theo id
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.Id == id);
            if (coupon == null)
            {
                _notification.Error("Coupon not found");
                return RedirectToAction("Index");
            }

            // Xóa mã giảm giá khỏi csdl
            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            _notification.Success("Coupon deleted successfully");
            return RedirectToAction("Index");
        }
    }
}
