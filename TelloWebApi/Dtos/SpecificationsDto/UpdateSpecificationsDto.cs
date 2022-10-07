namespace TelloWebApi.Dtos.SpecificationsDto
{
    public class UpdateSpecificationsDto
    {
       public int Id { get; set; }
        public int ProductId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
