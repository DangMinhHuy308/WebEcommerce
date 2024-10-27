using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Data;
using WebEcommerce.Models;
using WebEcommerce.ViewModels;
using X.PagedList.Extensions;

namespace WebEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SupplyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SupplyController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, INotyfService notyfService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _notification = notyfService;
        }
        [Authorize(Roles = "Admin,Author")]
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5;
            int pageNum = (page ?? 1);

            var supplier = await _context.Suppliers.ToListAsync();

            var listOfSupplierVM = supplier.Select(x => new SupplyVM()
            {
                Id = x.SupplierId,
                Name = x.CompanyName,
                Email = x.Email,
                Address = x.Address,
                Phone = x.Phone,
                CreatedDate = x.CreatedDate,
                Image = x.Logo
            }).ToList();

            // Phân trang danh sách CategoryVM
            var pagedSupplierVM = listOfSupplierVM.ToPagedList(pageNum, pageSize);

            return View(pagedSupplierVM);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateSupplyVM());
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Create(CreateSupplyVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var supply = new Supplier()
            {
                CompanyName = vm.Name,
                ContactPerson = vm.ContactPerson,
                Address = vm.Address,
                Email = vm.Email,
                Phone = vm.Phone,
                Description = vm.Description
                

            };


            //if (supply.CompanyName != null)
            //{
            //    string slug = vm.Name!.Trim();
            //    slug = slug.Replace(" ", "-");
            //    supply.Slug = slug + "-" + Guid.NewGuid();
            //}
            if (vm.Thumbnail != null)
            {
                supply.Logo = UploadImage(vm.Thumbnail);
            }
            await _context.Suppliers.AddAsync(supply);
            await _context.SaveChangesAsync();
            _notification.Success("Supply created successfully");
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supply = await _context.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id);
            if (supply == null)
            {
                _notification.Error("Supply not found");
                return View();
            }
            var vm = new CreateSupplyVM()
            {
                Id = supply.SupplierId,
                Name = supply.CompanyName,
                ContactPerson = supply.ContactPerson,
                Address = supply.Address,
                Email = supply.Email,
                Phone = supply.Phone,
                Description = supply.Description,
                Image = supply.Logo,
            };
            return View(vm);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Edit(CreateSupplyVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var supply = await _context.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == vm.Id);
            if (supply == null)
            {
                _notification.Error("Category not found");
                return View(vm);
            }
                supply.CompanyName = vm.Name;
                supply.ContactPerson = vm.ContactPerson;
                supply.Address = vm.Address;
                supply.Email = vm.Email;
                supply.Phone = vm.Phone;
                supply.Description = vm.Description;
            if (vm.Thumbnail != null)
            {
                supply.Logo = UploadImage(vm.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Edit successfully");
            return RedirectToAction("Index", "Supply", new { area = "Admin" });

        }

        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Find the category by ID
            var supply = await _context.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id);
            if (supply == null)
            {
                _notification.Error("Supply not found.");
                return RedirectToAction("Index", "Supply", new { area = "Admin" });
            }
            _context.Suppliers.Remove(supply);
            await _context.SaveChangesAsync();
            _notification.Success("Delete successfully");
            return RedirectToAction("Index", "Supply", new { area = "Admin" });
        }
        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;

        }
    }
}
