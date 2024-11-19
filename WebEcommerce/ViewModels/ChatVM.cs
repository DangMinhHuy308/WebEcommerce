namespace WebEcommerce.ViewModels
{
    public class ChatVM
    {
        public string CurrentUserId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string MessageContent { get; set; }
        public DateTime? When{ get; set; } = DateTime.Now;
        public string SelectedUserName { get; set; }
        public List<MessageBox> Messages { get; set; }
        public List<User> UserLists { get; set; }


    }

    public class User
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
    }

    public class MessageBox
    {
        public string FromUserId { get; set; }
        public string? FromUserAvatarUrl { get; set; } 
        public string MessageContent { get; set; }
        public DateTime SendTime { get; set; } = DateTime.Now;
        public string ToUserId { get; set; }
        public string? ToUserAvatarUrl { get; set; } 

    }
}
