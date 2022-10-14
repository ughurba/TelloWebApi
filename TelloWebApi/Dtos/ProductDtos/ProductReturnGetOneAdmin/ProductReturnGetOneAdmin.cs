using System;
using System.Collections.Generic;
using TelloWebApi.Models;

namespace TelloWebApi.Dtos.ProductDtos.ProductReturnGetOneAdmin
{
    public class ProductReturnGetOneAdmin
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double NewPrice { get; set; }
        public Nullable<double> OldPrice { get; set; }
        public int StockCount { get; set; }
        public string Description { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public List<Photo> ChildPhotos { get; set; }
        public List<Color> Colors { get; set; }
        public List<Storage> Storages { get; set; }
        public Photo MainPhoto { get; set; }


    }
}
