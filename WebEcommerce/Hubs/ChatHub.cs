using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebEcommerce.ViewModels;
namespace WebEcommerce.Hubs
{
    public class ChatHub: Hub
    {
        public async Task SendMessage(string user, string message)
        {
            /*// Gửi tin nhắn đến người nhận
            await Clients.User(message.ToUserId).SendAsync("ReceiveMessage", message);

            // Gửi lại tin nhắn cho người gửi để hiển thị ngay
            await Clients.User(message.FromUserId).SendAsync("ReceiveMessage", message);*/
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
