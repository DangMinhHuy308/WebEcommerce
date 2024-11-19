using Microsoft.AspNetCore.SignalR;

namespace WebEcommerce.Services
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity?.Name; 
        }
    }
}
