using System.Collections.Generic;

namespace TelloWebApi.Models
{
    public class Color
    {
      
        public int Id { get; set; }
        public string Code { get; set; }
        public List<ProductColor> ProductColors { get; set; }
    }
}
