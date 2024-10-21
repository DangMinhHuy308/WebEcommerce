using Microsoft.AspNetCore.Mvc;

namespace WebEcommerce.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
