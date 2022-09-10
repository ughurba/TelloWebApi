using System;
using System.Collections.Generic;
using TelloWebApi.Models;

namespace TelloWebApi.Dtos.ProductDtos.ProductReturnDto
{
    public class ProductReturnDto
    {
        public  int Id { get; set; }
        public bool inStock { get; set; }
        public double NewPrice { get; set; }
        public Nullable <double >OldPrice { get; set; }
        public  string Title { get; set; }
        public string CategoryTitle {get; set; }

        public string Description { get; set; }

        public string PhotoPath { get; set; }
        public bool isMainPhoto { get; set; }
        public List<Photo> Photos { get; set; }

    }
}
