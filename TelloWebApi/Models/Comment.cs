namespace TelloWebApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        public AppUser AppUser { get; set; }
        public Product Product { get; set; }
    }
}
