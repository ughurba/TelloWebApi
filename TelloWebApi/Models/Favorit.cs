using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelloWebApi.Models
{
    public class Favorit
    {
        public int Id { get; set; }
        public int PrdocutId { get; set; }
        public string AppUserId { get; set; }
    
    }
}
