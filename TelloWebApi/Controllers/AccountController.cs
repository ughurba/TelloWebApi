using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TelloWebApi.Data;
using TelloWebApi.Dtos.AccountDtos.LoginDto;
using TelloWebApi.Dtos.AccountDtos.RegisterDto;

using TelloWebApi.Dtos.AccountDtos.UpdateUserDto;
using TelloWebApi.Models;
using static TelloWebApi.Helper.Helper;

namespace TelloWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
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
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim("Name", user.Name));
            claims.Add(new Claim("Surname", user.Surname));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("Email", user.Email));
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var item in roles)
            {
                claims.Add(new Claim("Role", item));
            }
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
        //[HttpPost("googleLogin")]
        //public IActionResult GoogleLogin(GoogleLoginDto googleLoginDto )
        //{

        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            AppUser user = await _userManager.FindByEmailAsync(registerDto.Email);
            if (user != null)
            {
                return BadRequest("Bu e-mail istifadəçi artıq mövcuddur");
            }
            user = new AppUser
            {
                UserName = registerDto.Email,
                Name = registerDto.Name.ToLower(),
                Email = registerDto.Email,
                Surname = registerDto.Surname.ToLower(),
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    return BadRequest(item.Description);
                }
               
            }
            result = await _userManager.AddToRoleAsync(user, UserRoles.Member.ToString());
            return StatusCode(200);
        }

        [HttpGet("users")]

        public IActionResult GetUser()
        {
            List<AppUser> appUsers = _context.Users.ToList();
            return Ok(appUsers);

        }


        [HttpPut("updateUser")]
        public async Task <IActionResult> UpdateUser(UpdateUserDto updateUserDto)
        {
            AppUser user = await _userManager.FindByNameAsync(updateUserDto.UserName);
            user.Birthday = updateUserDto.Birthda;
            user.Name = updateUserDto.FirstName;
            user.Surname = updateUserDto.LastName;
            _context.SaveChanges();

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim("Name", user.Name));
            claims.Add(new Claim("Surname", user.Surname));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("Email", user.Email));
            claims.Add(new Claim("Birthda", user.Birthday));
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var item in roles)
            {
                claims.Add(new Claim("Role", item));
            }
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

        //[HttpGet("userprofile")]
        //[Authorize]
        //public async Task<IActionResult> GetProfile()
        //{
        //    AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    return Ok(new { Name = user.UserName });

        //}

        [HttpGet]
        public async Task CreateRole()
        {
            foreach (var item in Enum.GetValues(typeof(UserRoles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = item.ToString() });
                }
            }
        }
    }
}
