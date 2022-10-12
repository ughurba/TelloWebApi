namespace TelloWebApi.Models
{
    public class BasketItem
    {

        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string Code { get; set; }
        public int? Storage { get; set; }
        public double Sum { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public string Path { get; set; }
        public double TotalPrice { get; set; }
    }
}
