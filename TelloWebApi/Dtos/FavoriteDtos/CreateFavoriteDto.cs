namespace TelloWebApi.Dtos.FavoriteDtos
{
    public class CreateFavoriteDto
    {
        public int ProductId { get; set; }
        public  bool isFavorite { get; set; }
        public int favId { get; set; }    
    }
}
