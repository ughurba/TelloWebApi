using System;

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


    }
}
