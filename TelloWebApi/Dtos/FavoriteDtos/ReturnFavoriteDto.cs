using System;
using System.Collections.Generic;
using TelloWebApi.Models;

namespace TelloWebApi.Dtos.FavoriteDtos
{
    public class ReturnFavoriteDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<double> OldPrice { get; set; }
        public double NewPrice { get; set; }

        public bool isFavorite { get; set; } = false;


        public List<Photo> Photos { get; set; }
    
    }
}
