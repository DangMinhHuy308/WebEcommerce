﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Data;
using WebEcommerce.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebEcommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? id)
        {
            var product = _context.Products.AsQueryable();
            if (id.HasValue) { 
                product = product.Where(x => x.CategoryId == id.Value);
            }
            var result = product.Select(x => new ProductVM
            {
                Id = x.ProductId,
                Name = x.ProductName,
                Price = x.Price,
                Description = x.Description,
                Image = x.Image
            });
            return View(result);
        }
        public IActionResult Search(string? query)
        {
            var product = _context.Products.AsQueryable();
            if (query != null)
            {
                product = product.Where(x => x.ProductName.Contains(query));
            }
            var result = product.Select(x => new ProductVM
            {
                Id = x.ProductId,
                Name = x.ProductName,
                Price = x.Price,
                Image = x.Image
            });
            return View(result);
        }
        public IActionResult Detail(int id) 
        {
            var product = _context.Products
            .Include(x => x.Category) 
            .SingleOrDefault(x => x.ProductId == id);

            if (product == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            var result = new ProductDetailVM
            {
                Id = product.ProductId,
                Name = product.ProductName,
                Price = product.Price,
                Description = product.Description,
                Image = product.Image,
                CategoryName = product.Category?.CategoryName
            };

            return View(result); 
        }
    }
}
