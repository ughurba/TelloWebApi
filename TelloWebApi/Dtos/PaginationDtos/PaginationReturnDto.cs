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
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public Brand Brand { get; set; }
        public List<Color> Colors { get; set; }
        public List<Photo> Photos { get; set; }
        public List<Favorit> Favorits { get; set; }

    
    }
}
