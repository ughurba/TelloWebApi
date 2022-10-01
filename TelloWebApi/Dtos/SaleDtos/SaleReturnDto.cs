using System;
using System.Collections.Generic;
using TelloWebApi.Models;

namespace TelloWebApi.Dtos.SaleDtos
{
    public class SaleReturnDto
    {
        public  int Id { get; set; }
        public string Date{ get; set; }
        public  OrderStatus OrderStatus { get; set; }

        public double Total { get; set; }

        public List <Photo> Photos{ get; set; }
    }
}
