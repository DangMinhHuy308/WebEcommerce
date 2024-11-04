using Microsoft.AspNetCore.Mvc;
using WebEcommerce.Data;
using WebEcommerce.ViewModels;
using WebEcommerce.Helpers;
using Microsoft.AspNetCore.Authorization;
using WebEcommerce.Models;

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
            var item = cart.SingleOrDefault(x => x.Id == id);
            if (item == null)
            {
                var product = _context.Products.Find(id);
                if (product != null)
                {
                    cart.Add(new CartVM
                    {
                        Id = product.ProductId,
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
            var item = cart.SingleOrDefault(x => x.Id == id);
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
            if (!ModelState.IsValid) { return View(vm); }

            var invoice = new Invoice
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Address = vm.Address,
                Email = vm.Email,
                PhoneNumber = vm.PhoneNumber,
                OrderDate = vm.OrderDate,
                Notes = vm.Notes,
                PaymentMethod = vm.PaymentMethod,
                ShippingMethod = vm.ShippingMethod,
                StatusId = 0

            };
            _context.Add(invoice);
            _context.SaveChanges();
            foreach (var i in Cart)
            {
                var invoiceDetail = new InvoiceDetail
                {
                    InvoiceId = invoice.InvoiceId,
                    ProductId = i.Id,
                    Quantity = i.Quantity,
                    Price = i.Price
                };
                _context.InvoiceDetails.Add(invoiceDetail);
            }
            _context.SaveChanges();

            Cart.Clear();

            return RedirectToAction("OrderConfirmation", new {id = invoice.InvoiceId });
        }

    }
}
