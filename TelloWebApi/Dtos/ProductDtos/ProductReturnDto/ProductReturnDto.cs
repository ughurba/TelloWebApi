using System;
using System.Collections.Generic;
using TelloWebApi.Models;

namespace TelloWebApi.Dtos.ProductDtos.ProductReturnDto
{
    public class ProductReturnDto
    {
        public  string Title { get; set; }
        public Nullable <double>  OldPrice{ get; set; }
        public double NewPrice { get; set; }
        public List<Photo> Photos{ get; set; }
    }
}
