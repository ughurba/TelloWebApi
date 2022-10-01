using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelloWebApi.Data;
using TelloWebApi.Dtos.SaleDtos;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public SaleController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        [Autorize]
        public async Task<IActionResult> SaleStart(SaleCreateDto saleCreateDto)
        {
            string UserToken = HttpContext.Request.Headers["Authorization"].ToString();
            var userId = Helper.Helper.DecodeToken(UserToken);

            List<Product> dbProducts = _context.Products.Include(p => p.Photos).ToList();
            List<BasketItem> basketItems = _context.BasketItems.Include(b => b.Product).Where(b => b.AppUserId == userId).ToList();
            List<OrderItem> orderItems = new List<OrderItem>();
            AppUser user = await _userManager.FindByIdAsync(userId);

            Order order = new Order();
            //AppUser appUser = new AppUser
            //{
            //    Name = saleCreateDto.FirstName,
            //    Email = saleCreateDto.Email,
            //    Surname = saleCreateDto.LastName,
            //};
            order.AppUserId = userId;
            order.AppUser = user;
            order.Address = saleCreateDto.Address;
            order.Note = saleCreateDto.Courier;
            order.Building = saleCreateDto.Building;
            order.Mobile = saleCreateDto.Mobile;
            order.Cash = saleCreateDto.Cash;
            order.CreatedAt = DateTime.Now;
            order.OrderStatus = OrderStatus.Pending;

            await _context.Orders.AddAsync(order);

            foreach (var item in basketItems)
            {
                foreach (var product in dbProducts)
                {
                    if(product.Id == item.ProductId)
                    {
                        product.StockCount -= item.Count;
                    }
                    
                }
                OrderItem orderItem = new OrderItem()
                {
                    Count = item.Count,
                    Total = item.Sum,
                    Product = item.Product,
                    OrderId = order.Id,
                    Order = order,
                    ProductId = item.ProductId
                };
                await _context.AddAsync(orderItem);
                _context.BasketItems.Remove(item);
            }
            await _context.SaveChangesAsync();

            return StatusCode(200);
        }
    }
}
