using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelloWebApi.Data;
using TelloWebApi.Dtos.UserDto;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public AdminUserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet("getAllUser")]
        [Authorize]
        public async Task<IActionResult> getAllUsers()
        {
            List<AppUser> appUsers = _userManager.Users.ToList();
            List<ReturnUserDto> returnUserDtos = new List<ReturnUserDto>();
            foreach (var item in appUsers)
            {


                ReturnUserDto returnUserDto = new ReturnUserDto();
                returnUserDto.userRoles = await _userManager.GetRolesAsync(item);
                returnUserDto.Id = item.Id;
                returnUserDto.Surname = item.Surname;
                returnUserDto.Name = item.Name;
                returnUserDto.Birthday = item.Birthda;
                returnUserDto.Email = item.Email;
                returnUserDtos.Add(returnUserDto);

            }
            return Ok(returnUserDtos);

        }


    }
}
