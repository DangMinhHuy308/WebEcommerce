namespace WebEcommerce.Models
{
    public class Message
    {
        public int Id { get; set; }
        // Người gửi
        public string? FromUserId { get; set; }

        // Người nhận
        public string? ToUserId { get; set; }

        public string? Username{ get; set; }
        public string? MessageContent { get; set; }

        public DateTime? When { get; set; } = DateTime.Now;
        public virtual ApplicationUser? FromUser { get; set; }
        public virtual ApplicationUser? ToUser { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
