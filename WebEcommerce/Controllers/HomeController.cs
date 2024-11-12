using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebEcommerce.Data;
using WebEcommerce.Models;
using WebEcommerce.ViewModels;

namespace WebEcommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            // tao ra 1 danh sach cac id danh muc lay tu csdl
            var categoryIdsToDisplay = new List<int> { 1, 2, 3 };
            
            var categories = _context.Categories
                .Where(c => categoryIdsToDisplay.Contains(c.CategoryId)) //loc cac danh muc chi lay nhung danh muc co id nam trong danh sach categoryIdsToDisplay
                .Select(c => new CategoryWithProductVM
                {
                    CategoryName = c.CategoryName,
                    Products = c.Products
                    .Where(p=> p.IsActive)
                    .Select(p=> new ProductVM
                    {
                        Id = p.ProductId,
                        Name = p.ProductName,
                        Price = p.Price,
                        Image = p.Image
                    }).ToList()
                }).ToList();
            

            return View(categories);
        }
        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
