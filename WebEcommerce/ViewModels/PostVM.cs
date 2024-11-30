﻿namespace WebEcommerce.ViewModels
{
    public class PostVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? AuthorName { get; set; }
        public string? Description { get; set; }
        public string? ShortDescription { get; set; }
      
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public string? ThumbnailUrl { get; set; }
    }
}