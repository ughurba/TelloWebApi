using System;

namespace TelloWebApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public DateTime CreateTime { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        public AppUser AppUser { get; set; }
        public Product Product { get; set; }
    }
}
