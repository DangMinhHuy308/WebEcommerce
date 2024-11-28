using Microsoft.AspNetCore.Mvc;

namespace WebEcommerce.Controllers
{
	public class AboutController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
