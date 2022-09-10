using System;
using System.Collections.Generic;
using TelloWebApi.Models;

namespace TelloWebApi.Dtos.PaginationDtos
{
    public class PaginationReturnDto
    {
        public int Id { get; set; }
        public bool inStock { get; set; }
        public double NewPrice { get; set; }
        public Nullable<double> OldPrice { get; set; }
        public string Title { get; set; }
        public string CategoryTitle { get; set; }

        public string Description { get; set; }

        public List<Photo> Photos { get; set; }

    
    }
}
