namespace WebEcommerce.ViewModels
{
    public class ChatVM
    {
        // Thông tin về người dùng hiện tại và người đang chat
        public string? FromUserId { get; set; }
        public string? ToUserId { get; set; }
        public string? FromUserName { get; set; }
        public string? ToUserName { get; set; }
        public string? FromUserImage { get; set; }

        // Danh sách tất cả người dùng để hiển thị trong danh sách người dùng
        public List<UserList> Users { get; set; } = new List<UserList>();

        // Danh sách các tin nhắn của cuộc hội thoại hiện tại
        public List<MessageBox> Messages { get; set; } = new List<MessageBox>();
    }

    public class UserList
    {
        public string Id { get; set; }
        public string? UserName { get; set; } // Tên người dùng để hiển thị
        public string? Image { get; set; }     // Ảnh đại diện
        public bool IsOnline { get; set; }     // Trạng thái online/offline
        public DateTime? LastSeen { get; set; } // Thời gian lần cuối hoạt động
    }

    public class MessageBox
    {
        public string? FromUserName { get; set; }  // Tên người gửi
        public string? Content { get; set; }       // Nội dung tin nhắn
        public DateTime When { get; set; }         // Thời gian gửi tin nhắn
        public bool IsMyMessage { get; set; }      // Để xác định xem có phải tin nhắn của người dùng hiện tại hay không
    }
}
