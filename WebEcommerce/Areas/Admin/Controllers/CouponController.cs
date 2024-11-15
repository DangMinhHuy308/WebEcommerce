using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebEcommerce.Data;

namespace WebEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _context;
        [Authorize(Roles = "Admin,Author")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
