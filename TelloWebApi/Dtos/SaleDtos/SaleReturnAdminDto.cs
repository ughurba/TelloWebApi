using System.Collections.Generic;
using TelloWebApi.Models;

namespace TelloWebApi.Dtos.SaleDtos
{
    public class SaleReturnAdminDto
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }

        public double Total { get; set; }
        public string Adress { get; set; }
        public string Building { get; set; }
        public bool Cash { get; set; }
        public string Note { get; set; }

        public List<Photo> Photos { get; set; }
    }
}
