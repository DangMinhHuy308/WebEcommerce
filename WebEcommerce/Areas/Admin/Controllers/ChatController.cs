using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WebEcommerce.Data;
using WebEcommerce.Hubs;
using WebEcommerce.Models;
using WebEcommerce.ViewModels;

namespace WebEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        [Authorize(Roles = "Admin,Author")]
        [HttpGet]
        public async Task<IActionResult> Index(string userId)
        {
            /*var currentUser = await _userManager.GetUserAsync(User);

           
            // Lấy danh sách người dùng để hiển thị trong phần danh sách người dùng
            var users = await _context.ApplicationUsers
                .Where(u => u.Id != currentUser.Id)
                .Select(u => new UserList
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Image = u.Image
                })
                .ToListAsync();

            // Lấy danh sách tin nhắn giữa người dùng hiện tại và người dùng đã chọn
            var messages = await _context.Messages
                .Where(m => (m.FromUserId == currentUser.Id && m.ToUserId == userId) ||
                            (m.FromUserId == userId && m.ToUserId == currentUser.Id))
                .OrderBy(m => m.When)
                .Select(m => new MessageBox
                {
                    FromUserName = m.FromUser.UserName,
                    Content = m.MessageContent,
                    When = m.When ?? DateTime.Now,
                    IsMyMessage = m.FromUserId == currentUser.Id
                })
                .ToListAsync();

            // Tạo ViewModel ChatVM
            var vm = new ChatVM
            {
                FromUserId = currentUser.Id,
                FromUserName = currentUser.UserName,
                FromUserImage = currentUser.Image, 
                ToUserId = userId,
                ToUserName = users.FirstOrDefault(u => u.Id == userId)?.UserName,
                Users = users,
                Messages = messages
            };
            return View(vm);
*/          return View();
        }
        [HttpGet]
        public IActionResult GetMessages(string userId)
        {
            /*var currentUserId = _userManager.GetUserId(User);

            // Lấy tin nhắn giữa currentUserId và userId
            var messages = _context.Messages
                .Where(m => (m.FromUserId == currentUserId && m.ToUserId == userId) ||
                            (m.FromUserId == userId && m.ToUserId == currentUserId))
                .OrderBy(m => m.When)
                .Select(m => new MessageBox
                {
                    FromUserName = m.FromUser.UserName,
                    Content = m.MessageContent,
                    When = m.When ?? DateTime.Now,
                    IsMyMessage = m.FromUserId == currentUserId
                })
                .ToList();

            // Lấy thông tin người nhận
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Json(new
            {
                messagesHtml = RenderMessagesHtml(messages),
                userName = $"{user.FirstName} {user.LastName}",
                userImage = user.Image
            });*/
            return View();
        }
        /*private string RenderMessagesHtml(List<MessageBox> messages)
        {
            var sb = new StringBuilder();

            foreach (var message in messages)
            {
                var alignment = message.IsMyMessage ? "text-right" : "";
                var messageClass = message.IsMyMessage ? "my-message" : "other-message";

                sb.Append($@"
            <li class='clearfix {alignment}'>
                <div class='message-data {alignment}'>
                    <span class='message-data-time'>{message.When:hh:mm tt, MMM d}</span>
                </div>
                <div class='message {messageClass}'>
                    {message.Content}
                </div>
            </li>");
            }

            return sb.ToString();
        }*/
        [HttpPost]
        public async Task<IActionResult> SendMessage(string toUserId, string messageContent)
        {
            /*var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || string.IsNullOrEmpty(toUserId) || string.IsNullOrEmpty(messageContent))
            {
                return BadRequest("Invalid message data.");
            }

            // Tạo tin nhắn mới và lưu vào cơ sở dữ liệu
            var message = new Message
            {
                FromUserId = currentUser.Id,
                ToUserId = toUserId,
                MessageContent = messageContent,
                When = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Tạo đối tượng ChatMessage để gửi qua SignalR
            var chatMessage = new
            {
                FromUserId = currentUser.Id,
                ToUserId = toUserId,
                FromUserName = currentUser.UserName,
                FromUserImage = currentUser.Image, // Giả sử ApplicationUser có trường Image
                MessageContent = messageContent,
                When = message.When
            };

            // Gửi tin nhắn tới người dùng đích qua SignalR
            await _hubContext.Clients.User(toUserId).SendAsync("ReceiveMessage", chatMessage);

            // Đồng thời gửi lại tin nhắn cho người gửi
            await _hubContext.Clients.User(currentUser.Id).SendAsync("ReceiveMessage", chatMessage);

            return View();*/
            return View();
        }


    }
}
