using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TelloWebApi.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Birthda { get; set; }
   
        public List <Comment> Comments { get; set; }
        public List <Favorit> Favorits { get; set; }

    }
}
