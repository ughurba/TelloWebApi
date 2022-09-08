using System.Collections.Generic;

namespace TelloWebApi.Models
{
    public class Brand
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public List<Product> Products { get; set; }
    }
}
