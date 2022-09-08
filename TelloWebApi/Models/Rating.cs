namespace TelloWebApi.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int OneStart { get; set; }
        public int TwoStart { get; set; }
        public int ThreeStart { get; set; }
        public int FourStart { get; set; }
        public int FiveStart { get; set; }
        public double Avarge { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
