using System.Collections.Generic;

namespace TelloWebApi.Models
{
    public class Storage
    {
        public int Id{ get; set; }
        public int Value{ get; set; }
        public List <ProductStorage> ProductStorages{ get; set; }
    }
}
