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
        public IActionResult Index()
        {
            return View(Cart);
        }
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(x => x.ProductId == id);
            if (item == null)
            {
                var product = _context.Products.Find(id);
                if (product != null)
                {
                    cart.Add(new CartVM
                    {
                        ProductId = product.ProductId,
                        Name = product.ProductName,
                        Image = product.Image,
                        Price = product.Price,
                        Quantity = quantity
                    });
                }
            }
            else
            {
                item.Quantity += quantity;
            }

            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            return RedirectToAction("Index");
        }
        public IActionResult RemoveToCart(int id)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(x => x.ProductId == id);
            if (item != null)
            {

                cart.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, cart);
            }
            return RedirectToAction("Index");

        }
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("MYCART");
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
       public IActionResult CheckOut()
       {
            if(Cart.Count == 0)
            {
                return Redirect("/");
            }
            return View(Cart);
       }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CheckOut(CheckoutVM vm)
        {
            var UserEmail = User.FindFirstValue(ClaimTypes.Email);
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
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
                ShippingFee = 10.0f, 
                StatusId = 1, 
                Notes = vm.Notes,
                Code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9)
            };
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
            var receiver = UserEmail;
            var subject = "Đặt hàng thành công";
            var message ="Đặt hàng thành công";
            await _emailSender.SendEmailAsync(receiver, subject, message);
            _context.Invoices.Add(invoice);
            _context.SaveChanges();
            ClearCart();
            return RedirectToAction("Success");
        }
        public IActionResult Success()
        {
            return View();
        }
        [Authorize]
        public IActionResult PaymentCallBack(CheckoutVM vm)
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }
            // Tạo một bản ghi Invoice mới
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
            _context.Invoices.Add(invoice);
            _context.SaveChanges(); 

           
            HttpContext.Session.Remove(CART_KEY);

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("Success");
        }

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

    }
}
