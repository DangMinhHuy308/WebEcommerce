using Microsoft.AspNetCore.Mvc;

namespace WebEcommerce.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
