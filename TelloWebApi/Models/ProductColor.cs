namespace TelloWebApi.Models
{
    public class ProductColor
    {
        public int Id{ get; set; }
        public  int ProductId { get; set; }
        public int ColorId{ get; set; }
        public Product Product { get; set; }
        public Color Colors { get; set; }
    }
}
