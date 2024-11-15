using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebEcommerce.Data;
using WebEcommerce.Hubs;
using WebEcommerce.Models;
using WebEcommerce.ViewModels;

namespace WebEcommerce.Controllers
{
    public class ChatController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> Index()
        {
            

            return View();
        }
        /*[HttpPost]
        public async Task<IActionResult> SendMessage(string messageContent, string toUserId)
        {
            var fromUser = await _userManager.GetUserAsync(User);
            if (fromUser == null) return Unauthorized();

            // Tạo tin nhắn mới
            var message = new Message
            {
                FromUserId = fromUser.Id,
                ToUserId = toUserId,
                MessageContent = messageContent,
                When = DateTime.Now
            };

            // Lưu vào database
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Gửi thông điệp qua SignalR
            await _hubContext.Clients.User(toUserId).SendAsync("ReceiveMessage", fromUser.UserName, messageContent);

            return RedirectToAction("Index");
        }*/
    }
}
