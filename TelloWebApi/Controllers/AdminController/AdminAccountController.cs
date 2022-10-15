using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using TelloWebApi.Data;
using TelloWebApi.Dtos.AccountDtos.LoginDto;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public AdminAccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            AppUser user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return NotFound("İstifadəçi tapılmadı");
            }
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return BadRequest("Xəta baş verib: The username or password you entered is incorrect. Please check the username, re-type the password, and try again.");
            }
            List<Claim> claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(Helper.Helper.UserRoles.Admin.ToString()))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                claims.Add(new Claim("Name", user.Name));
                claims.Add(new Claim("Surname", user.Surname));
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                claims.Add(new Claim("Email", user.Email));

                string secreKey = "2ee9d5f7-3dd0-4a06-a341-7f7cdc1a7f9c";
                SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secreKey));
                SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddHours(50),
                    SigningCredentials = credentials,
                    Audience = "http://localhost:33033/",
                    Issuer = "http://localhost:33033/"

                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new { token = tokenHandler.WriteToken(token) });
            }
           
            return BadRequest("Xəta baş verib: Admin deyilsiniz");

        }


    }
}
