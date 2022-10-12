using System;
using System.Collections.Generic;

namespace TelloWebApi.Models
{
    public class Product
    {
       
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<double> OldPrice { get; set; }
        public double NewPrice { get; set; }
        public bool isDeleted { get; set; }
        public bool inStock { get; set; }
        public int StockCount { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Photo> Photos { get; set; }
        public List<ProductColor> ProductColors { get; set; }
        public List<ProductDetails> ProductDetails { get; set; }
        public List<ProductStorage> ProductStorages { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public List<Rating> Ratings { get; set; }
        public List <Favorit> Favorits{ get; set; }

    }
}
