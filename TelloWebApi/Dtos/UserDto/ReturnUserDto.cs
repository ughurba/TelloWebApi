using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TelloWebApi.Dtos.UserDto
{
    public class ReturnUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public IList<string> userRoles{ get; set; }

    }
}
