using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebEcommerce.ViewModels;
namespace WebEcommerce.Hubs
{
    public class ChatHub: Hub
    {
        public async Task SendMessage(ChatVM message)
        {
            // Gửi tin nhắn đến người nhận
            await Clients.User(message.ToUserId).SendAsync("ReceiveMessage", message);

            // Gửi lại tin nhắn cho người gửi để hiển thị ngay
            await Clients.User(message.FromUserId).SendAsync("ReceiveMessage", message);
        }
    }
}
