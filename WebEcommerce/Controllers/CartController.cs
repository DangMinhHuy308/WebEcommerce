using Microsoft.AspNetCore.Mvc;
using WebEcommerce.Data;
using WebEcommerce.ViewModels;
using WebEcommerce.Helpers;
using Microsoft.AspNetCore.Authorization;
using WebEcommerce.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Services;

namespace WebEcommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IVnPayService _vnPayService;
        private IEmailSender _emailSender;

        public CartController(ApplicationDbContext context, IVnPayService vnPayService, IEmailSender emailSender) {
            _context = context;
            _vnPayService = vnPayService;
            _emailSender = emailSender;

        }
        const string CART_KEY = "MYCART";
        public List<CartVM> Cart => HttpContext.Session.Get<List<CartVM>>(MySetting.CART_KEY) ?? new List<CartVM>();
       
        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            return View(Cart);
        }

        // Thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(int id, int quantity = 1, string? couponCode = null)
        {
            var cart = Cart;
            // Kiểm tra sản phẩm đã có trong giỏ chưa
            var item = cart.SingleOrDefault(x => x.ProductId == id);
           
            if (item == null)
            {
                // Lấy thông tin sản phẩm từ cơ sở dữ liệu
                var product = _context.Products.Find(id);
                // Nếu sản phẩm tồn tại
                if (product != null)
                {
                    cart.Add(new CartVM
                    {
                        ProductId = product.ProductId,
                        Name = product.ProductName,
                        Image = product.Image,
                        Price = product.Price,
                        Quantity = quantity,
                        CouponCode = couponCode
                    });
                }
            }
            else
            {
                item.Quantity += quantity;
                
            }
            // Lưu giỏ hàng vào session
            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult RemoveToCart(int id)
        {
            var cart = Cart;
            // Tìm sản phẩm trong giỏ
            var item = cart.SingleOrDefault(x => x.ProductId == id);
            if (item != null)
            {

                cart.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }

        // Xoa tat ca san pham trong gio hang
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("MYCART");
            return RedirectToAction("Index");
        }

        // Kiểm tra và xác nhận giỏ hàng khi người dùng đã đăng nhập
        [Authorize]
        [HttpGet]
       public IActionResult CheckOut()
       {
            // Nếu giỏ hàng rỗng, chuyển hướng về trang chủ
            if (Cart.Count == 0)
            {
                return Redirect("/");
            }
            return View(Cart);
       }

        // Xử lý thông tin thanh toán khi người dùng gửi form xác nhận
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckOut(CheckoutVM vm)
        {
            // Lấy email người dùng từ Claims
            var UserEmail = User.FindFirstValue(ClaimTypes.Email);
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Khởi tạo random để tạo mã đơn hàng
            Random rd = new Random();
            var invoice = new Invoice
            {

                ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier), // Lấy ID người dùng
                OrderDate = DateTime.Now,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                PhoneNumber = vm.PhoneNumber,
                Email = vm.Email,
                Address = vm.Address,
                PaymentMethod = vm.PaymentMethod, 
                ShippingMethod = vm.ShippingMethod, 
                RequiredDate = DateTime.Now.AddDays(3),
                ShippingFee = 10.0f, 
                StatusId = 1, 
                Notes = vm.Notes,
                Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9)
            };

            // Xử lý thanh toán VNPay nếu chọn phương thức này
            if (vm.PaymentMethod == "VNPay")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    OrderId = new Random().Next(1000, 100000),
                    FullName = $"{vm.FirstName} {vm.LastName}",
                    Description = $"Thanh toán đơn hàng #{invoice.InvoiceId}",
                    Amount = (double)(Cart.Sum(x => x.Price * x.Quantity) ?? 0),
                    CreatedDate = DateTime.Now
                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
            }

            // Áp dụng mã giảm giá nếu có
            var couponCode = Cart.FirstOrDefault()?.CouponCode;
            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                var coupon = _context.Coupons.SingleOrDefault(c => c.Name == couponCode && c.DateEnd >= DateTime.Now);
                if (coupon != null)
                {
                    // Gán CouponId cho hóa đơn
                    invoice.CouponId = coupon.Id; 
                    // Tính toán tổng tiền với giảm giá
                    double discountAmount = (double)(coupon.Price ?? 0); 
                    double totalAmount = (double)(Cart?.Sum(x => x.Price * x.Quantity) ?? 0) - discountAmount;
                }
            }

            // Thêm chi tiết hóa đơn cho mỗi sản phẩm trong giỏ hàng
            foreach (var item in Cart)
            {
                var invoiceDetail = new InvoiceDetail
                {
                    ProductId = item.ProductId,
                    InvoiceId = invoice.InvoiceId,
                    Quantity = item.Quantity,
                    Price = item.Price,

                };

                invoice.InvoiceDetails.Add(invoiceDetail);
            }

            // Gửi email xác nhận đơn hàng
            var receiver = UserEmail;
            var subject = "Đặt hàng thành công";
            var message ="Đặt hàng thành công";
            await _emailSender.SendEmailAsync(receiver, subject, message);

            // Lưu hóa đơn vào cơ sở dữ liệu
            _context.Invoices.Add(invoice);
            _context.SaveChanges();
            ClearCart(); 
            return RedirectToAction("Success");
        }

        // Trang thành công sau khi đặt hàng
        public IActionResult Success()
        {
            return View();
        }

        // Xử lý callback từ VNPay sau khi thanh toán
        [Authorize]
        public IActionResult PaymentCallBack(CheckoutVM vm)
        {
            // Lấy phản hồi từ VNPay
            var response = _vnPayService.PaymentExecute(Request.Query);
            // Kiểm tra kết quả thanh toán
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            // Tạo hóa đơn mới nếu thanh toán thành công
            var invoice = new Invoice
            {
                ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                OrderDate = DateTime.Now,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                PhoneNumber = vm.PhoneNumber,
                Email = vm.Email,
                Address = vm.Address,
                PaymentMethod = "VNPay", 
                ShippingMethod = "Fast", 
                ShippingFee = 10.0f, 
                StatusId = 1, 
                Notes = vm.Notes, 
                Code = "DH" + new Random().Next(1000, 9999)
            };

            // Thêm chi tiết đơn hàng vào hóa đơn (invoice)
            foreach (var item in Cart)
            {
                var invoiceDetail = new InvoiceDetail
                {
                    ProductId = item.ProductId,
                    InvoiceId = invoice.InvoiceId,
                    Quantity = item.Quantity,
                    Price = item.Price,

                };
                invoice.InvoiceDetails.Add(invoiceDetail);
            }

            // Lưu hóa đơn vào cơ sở dữ liệu
            _context.Invoices.Add(invoice);
            _context.SaveChanges(); 
            HttpContext.Session.Remove(CART_KEY);
            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("Success");
        }

        // Trang thất bại khi thanh toán VNPay không thành công
        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        // Áp dụng mã giảm giá
        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
            // Kiểm tra mã giảm giá hợp lệ
            var coupon = _context.Coupons.SingleOrDefault(c => c.Name == couponCode && c.DateEnd >= DateTime.Now);
            if (coupon == null)
            {
                TempData["Error"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Index");
            }

            var cart = Cart;
            // Áp dụng giảm giá cho từng sản phẩm trong giỏ hàng
            foreach (var item in cart)
            {
                item.Price -= coupon.Price; // Trừ tiền từ giá sản phẩm
                if (item.Price < 0) // Đảm bảo giá không âm
                {
                    item.Price = 0;
                }
                item.CouponCode = couponCode; // Lưu mã giảm giá vào sản phẩm
            }

            // Cập nhật giỏ hàng trong session
            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            TempData["Success"] = "Mã giảm giá đã được áp dụng thành công!";
            return RedirectToAction("Index");
        }

    }
}
