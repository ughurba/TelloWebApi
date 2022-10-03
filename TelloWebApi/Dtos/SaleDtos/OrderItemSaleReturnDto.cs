using System.Collections.Generic;
using TelloWebApi.Models;

namespace TelloWebApi.Dtos.SaleDtos
{
    public class OrderItemSaleReturnDto
    {
        public int Id { get; set; }
        public double Total { get; set; }
        public int Count { get; set; }
        public string Code { get; set; }
        public int Storage { get; set; }
        public string Title { get; set; }
        public List<Photo> Photos{ get; set; }

    }
}
