using System;
using System.Collections.Generic;
using TelloWebApi.Models;

namespace TelloWebApi.Dtos.ProductDtos.ProductReturnAdminDto
{
    public class ProductReturnAdminDto
    {
        public int Id { get; set; }
       public List<Photo> Photos { get; set; } 
        public string Title { get; set; }
        public double NewPrice { get; set; }
        public Nullable<double> OldPrice { get; set; }
        public int StockCount { get; set; }
        public string Description { get; set; }


    }
}
