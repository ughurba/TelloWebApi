namespace TelloWebApi.Dtos.SaleDtos
{
    public class SaleCreateDto
    {
        public string  Address{ get; set; }
        public string Building { get; set; }
        public string Courier { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName{ get; set; }
        public string Mobile { get; set; }
        public bool Cash { get; set; }
    }
}
