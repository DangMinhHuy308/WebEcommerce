using Microsoft.AspNetCore.Mvc;
using WebEcommerce.Helpers;
using WebEcommerce.ViewModels;

namespace WebEcommerce.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartVM>>(MySetting.CART_KEY) ?? new List<CartVM>();

            return View("Cart", new CartIconVM
            {
                Quantity = cart.Sum(p => p.Quantity),
                Total = cart.Sum(p => p.TotalPrice)
            });
        }
    }
}
