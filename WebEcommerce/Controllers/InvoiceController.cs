using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebEcommerce.Data;
using WebEcommerce.ViewModels;

namespace WebEcommerce.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public InvoiceController (ApplicationDbContext context)
        {
            _context = context;
        }
        // Theo dõi đơn hàng của người dùng
        public async Task<IActionResult> Tracking()
        {
            // Lấy ID người dùng hiện tại từ claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Lấy danh sách hóa đơn của người dùng, bao gồm thông tin chi tiết sản phẩm trong từng hóa đơn
            var invoices = await _context.Invoices
                .Where(i => i.ApplicationUserId == userId) // Lọc hóa đơn của người dùng hiện tại
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                .ToListAsync();
            // Chuyển đổi dữ liệu hóa đơn thành ViewModel để dễ dàng hiển thị trong View
            var viewModel = invoices.Select(i => new OrderTrackingVM
            {
                InvoiceId = i.InvoiceId,
                Code = i.Code,
                OrderDate = i.OrderDate,
                DeliveryDate = i.DeliveryDate,
                StatusId = i.StatusId,
                Items = i.InvoiceDetails.Select(d => new OrderItemVM
                {
                    ProductName = d.Product.ProductName,
                    Quantity = d.Quantity ?? 0,
                    Price = d.Price ?? 0,
                    Discount = d.Discount
                }).ToList()
            }).ToList();

            return View(viewModel);
        }
    }
}
