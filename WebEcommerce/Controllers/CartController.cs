using Microsoft.AspNetCore.Mvc;
using WebEcommerce.Data;
using WebEcommerce.ViewModels;
using WebEcommerce.Helpers;
using Microsoft.AspNetCore.Authorization;
using WebEcommerce.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace WebEcommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context) {
            _context = context;
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
        public IActionResult CheckOut(CheckoutVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var invoice = new Invoice
            {
                ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier), // Lấy ID người dùng
                OrderDate = DateTime.Now,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                PhoneNumber = vm.PhoneNumber,
                Email = vm.Email,
                Address = vm.Address,
                PaymentMethod = "COD", 
                ShippingMethod = "Fast", 
                ShippingFee = 10.0f, 
                StatusId = 1, 
                Notes = vm.Notes,
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

            Cart.Clear();

            return RedirectToAction("Success");
        }
        public IActionResult Success()
        {
            return View();
        }


    }
}
