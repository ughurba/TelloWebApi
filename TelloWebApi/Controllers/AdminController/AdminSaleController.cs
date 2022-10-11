using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public IActionResult GetAllOrder()
        {

  

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
        [HttpGet("OrderItem")]
        [Authorize]
        public IActionResult GetAllOrderItem(int orderId)
        {
            List<OrderItemSaleReturnDto> OrderItemSaleReturnDto = _context.OrderItems
                 .Where(x => x.OrderId == orderId)
                 .Include(x => x.Product)
                 .Select(x => new OrderItemSaleReturnDto
                 {
                     Id = x.Id,
                     Title = x.Product.Title,
                     Code = x.Code,
                     Count = x.Count,
                     Storage = x.Storage,
                     Total = x.Total,
                     Photos = x.Product.Photos
                 }).ToList();

            return Ok(OrderItemSaleReturnDto);
        }
        [HttpPut]
        [Authorize]
        public IActionResult UpdateOrderStatus(int orderId,int orderStatus)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == orderId);
            order.OrderStatus = (OrderStatus)orderStatus;
            _context.SaveChanges();
            return StatusCode(200);
        }
    }
}
