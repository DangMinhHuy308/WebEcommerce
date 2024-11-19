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
        // Hiển thị cuộc trò chuyện giữa người dùng và quản lý
        public async Task<IActionResult> Index(string userId, string toUserId = "admin")
        {

            // Fetch messages between the user and admin
            var messages = _context.Messages
                .Where(m => (m.FromUserId == userId && m.ToUserId == "admin") || (m.ToUserId == userId && m.FromUserId == "admin"))
                .OrderBy(m => m.When)
                .Select(m => new MessageBox
                {
                    FromUserId = m.FromUserId,
                    ToUserId = m.ToUserId,
                    MessageContent = m.MessageContent,
                    FromUserAvatarUrl = m.FromUser.Image,
                    ToUserAvatarUrl = m.ToUser.Image
                })
                .ToList();


            // Fetch all users (excluding admin for the customer)
            var users = _context.Users
                .Where(u => u.Id != userId) // Assuming customers can only message the manager (admin)
                .Select(u => new User
                {
                    Id = u.Id,
                    Name = u.UserName,
                    Image = ((ApplicationUser)u).Image
                }).ToList();
            var selectedUser = users.FirstOrDefault(u => u.Id == toUserId);

            // Set up the ChatVM model
            var chatVM = new ChatVM
            {
                CurrentUserId = userId,
                Messages = messages.Select(m => new MessageBox
                {
                    FromUserId = m.FromUserId,
                    ToUserId = m.ToUserId,
                    MessageContent = m.MessageContent,
                    
                }).ToList(),
                UserLists = users,
                SelectedUserName = selectedUser?.Name ?? "Admin"
            };

            return View(chatVM);
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(string fromUserId, string toUserId, string messageContent)
        {
            // Send the message to the client side using SignalR
            await _hubContext.Clients.User(toUserId).SendAsync("ReceiveMessage", fromUserId, messageContent);

            // Save the message to the database
            var message = new Message
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                MessageContent = messageContent,
                When = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Redirect to the conversation page
            return RedirectToAction("Index", new { userId = fromUserId });
        }
    }
}
