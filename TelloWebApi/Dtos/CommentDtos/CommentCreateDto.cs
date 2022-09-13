namespace TelloWebApi.Dtos.CommentDtos
{
    public class CommentCreateDto
    {
        public string AppUserId { get; set; }
        public  int ProductId  { get; set; }
        public string Content { get; set; }
    }
}
