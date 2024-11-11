using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEcommerce.Data;
using WebEcommerce.Models;
using WebEcommerce.ViewModels;
using X.PagedList.Extensions;

namespace WebEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public InvoiceController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,INotyfService notyfService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _notification = notyfService;
        }
        // Hiển thị danh sách hóa đơn
        [Authorize(Roles = "Admin,Author")]
        [HttpGet]
        public async Task<IActionResult> Index(int? page) 
        {
            int pageSize = 5;
            int pageNum = page ?? 1;
            var invoices = await _context.Invoices
                .Include(i => i.InvoiceDetails) // Bao gồm thông tin hóa đơn chi tiết liên quan
                .ToListAsync();
            var listOfInvoice = invoices.Select(x => new InvoiceVM()
            {
                Id = x.InvoiceId,
                Code = x.Code,
                FirstName = x.FirstName,
                LastName = x.LastName,
                //Price = InvoiceDetail.Price,
                Address = x.Address,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                PaymentMethod = x.PaymentMethod,
                OrderDate = x.OrderDate,
                DeliveryDate = x.DeliveryDate,
                ShippingMethod = x.ShippingMethod,
                ShippingFee = x.ShippingFee,
                StatusId = x.StatusId,
                Notes = x.Notes,
                InvoiceDetailId = x.InvoiceDetails.FirstOrDefault()?.Id

            }).ToList();
            var pagedInvoiceVM = listOfInvoice.ToPagedList(pageNum, pageSize);
            return View(pagedInvoiceVM);
        }
        // Cập nhập hóa đơn
        [Authorize(Roles = "Admin,Author")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(x => x.InvoiceId == id);
            if(invoice == null)
            {
                _notification.Error("Invoice not found");   
                return View();
            }
            var vm = new InvoiceVM()
            {
                Id = invoice.InvoiceId,
                Code = invoice.Code,
                FirstName = invoice.FirstName,
                LastName = invoice.LastName,
                Address = invoice.Address,
                PhoneNumber = invoice.PhoneNumber,
                Email = invoice.Email,
                PaymentMethod = invoice.PaymentMethod,
                OrderDate = invoice.OrderDate,
                DeliveryDate = invoice.DeliveryDate,
                ShippingMethod = invoice.ShippingMethod,
                ShippingFee = invoice.ShippingFee,
                StatusId = invoice.StatusId,
                Notes = invoice.Notes,
            };
            return View(vm);
        }
        [Authorize(Roles = "Admin,Author")]
        [HttpPost] 
        public async Task<IActionResult> Edit(InvoiceVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var invoice = await _context.Invoices.FirstOrDefaultAsync(x => x.InvoiceId == vm.Id);
            if (invoice == null)
            {
                _notification.Error("Invoice not found");
                return View(vm);
            }
            invoice.Code = vm.Code;
            invoice.FirstName = vm.FirstName;
            invoice.LastName = vm.LastName;
            //InvoiceDetail.Price = vm.Price;
            invoice.Address = vm.Address;
            invoice.PhoneNumber = vm.PhoneNumber;
            invoice.PaymentMethod = vm.PaymentMethod;
            invoice.OrderDate = vm.OrderDate;
            invoice.DeliveryDate = vm.DeliveryDate;
            invoice.ShippingMethod = vm.ShippingMethod;
            invoice.ShippingFee = vm.ShippingFee;
            invoice.StatusId = vm.StatusId;
            invoice.Notes = vm.Notes;
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            _notification.Success("Edit successfully");
            return RedirectToAction("Index");

        }
       // Xóa hóa đơn
       //[Authorize(Roles = "Admin,Author")]
       //[HttpPost]
       // public async Task<IActionResult> Delete(int id)
       // {
       //     var invoice = await _context.Invoices
       //         .Include(i => i.InvoiceDetails) // Include related InvoiceDetails
       //         .FirstOrDefaultAsync(x => x.InvoiceId == id);

       //     if (invoice == null)
       //     {
       //         _notification.Error("Invoice not found");
       //         return RedirectToAction("Index", "Invoice", new { area = "Admin" });
       //     }

       //     // Remove related InvoiceDetails first
       //     if (invoice.InvoiceDetails != null && invoice.InvoiceDetails.Any())
       //     {
       //         _context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);
       //     }

       //     _context.Invoices.Remove(invoice);
       //     await _context.SaveChangesAsync();

       //     _notification.Success("Delete successfully");
       //     return RedirectToAction("Index");
       // }
        [Authorize(Roles = "Admin,Author")]
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            // Tìm hóa đơn chi tiết theo ID
            var invoiceDetail = await _context.InvoiceDetails
                .Include(id => id.Invoice) // Bao gồm thông tin hóa đơn liên quan
                .Include(id => id.Product)  // Bao gồm thông tin sản phẩm liên quan
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoiceDetail == null)
            {
                _notification.Error("Không tìm thấy hóa đơn.");
                return View();
            }

            var vm = new InvoiceDetailVM
            {
                Price = invoiceDetail.Price,
                Quantity = invoiceDetail.Quantity,
                Code = invoiceDetail.Invoice?.Code,
                ProductName = invoiceDetail.Product?.ProductName
            };

            return View(vm); 
        }
    }
}
