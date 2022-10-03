using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TelloWebApi.Data;
using TelloWebApi.Dtos.SaleDtos;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminSaleController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public AdminSaleController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("AllOrder")]
        [Autorize]
        public IActionResult GetAllOrder()
        {

            //string UserToken = HttpContext.Request.Headers["Authorization"].ToString();
            //var userId = Helper.Helper.DecodeToken(UserToken);

            List<SaleReturnAdminDto> saleReturnAdminDto = _context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Photos)
                .Include(x => x.AppUser)
                .Select(x => new SaleReturnAdminDto
                {
                    Id = x.Id,
                    Date = x.CreatedAt.ToString("MM/dd/yyyy HH:mm"),
                    Mobile = x.Mobile,
                    OrderStatus = x.OrderStatus,
                    Photos = x.OrderItems.SelectMany(x => x.Product.Photos).ToList(),
                    Total = x.OrderItems.Select(x => x.Total).Sum(),
                    UserName = x.AppUser.Name,
                    Adress = x.Address,
                    Building = x.Building,
                    Cash = x.Cash,
                    Note = x.Note
                })
                .ToList();

            return Ok(saleReturnAdminDto);
        }
    }
}
