namespace TelloWebApi.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Path{ get; set; }
        public bool IsMain{ get; set; }
        public int ProductId{ get; set; }
       public Product Product{ get; set; }
    }
}
