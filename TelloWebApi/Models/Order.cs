using System;
using System.Collections.Generic;

namespace TelloWebApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string Building { get; set; }
        public string Mobile { get; set; }
        public bool Cash { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
    public enum OrderStatus
    {
        Pending,
        Shipped,
    }
}

