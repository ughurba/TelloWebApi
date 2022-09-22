using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelloWebApi.Data;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetBasketItems()
        {
            double total = 0;
            string UserToken = HttpContext.Request.Headers["Authorization"].ToString();
            var userId = Helper.Helper.DecodeToken(UserToken);
            var obj = new object();


            List<BasketItem> baskets = _context.BasketItems
                .Include(b => b.Product)
                .ThenInclude(b=>b.ProductColors)
                .Include(b=>b.Product)
                .ThenInclude(p=>p.ProductStorages)
                .Include(u => u.AppUser)
                .Where(b => b.AppUserId == userId && !b.Product.isDeleted)
                .ToList();
            foreach (var item in baskets)
            {
                total += item.Sum;
            }
            obj = new
            {
                basketItems = baskets,
                total = total,

            };

            return Ok(obj);
        }
        [HttpPost]
        [Authorize]

        public async Task<IActionResult> AddItem(int? productId,int? colorId,int? storageId)
        {
            double total = 0;
            var obj = new object();
            Product dbProduct = _context.Products
                .Include(p => p.Photos)
                .FirstOrDefault(p => p.Id == productId);
            Photo productImage = dbProduct.Photos.FirstOrDefault(p => p.ProductId == productId && p.IsMain);
            ProductColor color = _context.ProductColors.Include(c=>c.Colors).FirstOrDefault(p => p.ColorId == colorId);
            ProductStorage storage = _context.ProductStorages.Include(s=>s.Storage).FirstOrDefault(p => p.StorageId == storageId);
            BasketItem basketItem = new BasketItem();


            string UserToken = HttpContext.Request.Headers["Authorization"].ToString();
            var userId = Helper.Helper.DecodeToken(UserToken);
            BasketItem isExist = _context.BasketItems.Include(b => b.Product).FirstOrDefault(b => b.ProductId == dbProduct.Id && b.AppUserId == userId);
            if (isExist == null)
            {
                basketItem.Code = color.Colors.Code;
                basketItem.Storage = storage.Storage.Value;
                basketItem.ProductId = dbProduct.Id;
                basketItem.Price = dbProduct.NewPrice;
                basketItem.AppUserId = userId;
                basketItem.Product = dbProduct;
                basketItem.Count = 1;
                basketItem.Path = productImage.Path;
                basketItem.Sum = basketItem.Count * dbProduct.NewPrice;
                await _context.AddAsync(basketItem);
            }
            else
            {
                if (isExist.Count <= isExist.Product.StockCount)
                {
                    isExist.Count++;
                    isExist.Sum = isExist.Count * isExist.Price;
                }

            }


            await _context.SaveChangesAsync();
            List<BasketItem> basketItems = _context.BasketItems.Include(p=>p.Product).Where(b => b.AppUserId == userId).ToList();

         
            foreach (var item in basketItems)
            {
                total += item.Sum;
            }
            obj = new
            {
                basketItems = basketItems,  
                total = total,

            };


            return Ok(obj);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Plus(int id)
        {
            double total = 0;
            Product dbProduct = _context.Products.Include(p => p.Photos).FirstOrDefault(p => p.Id == id);
            List<object> frontBaskets = new List<object>();
            var obj = new object();
            string UserToken = HttpContext.Request.Headers["Authorization"].ToString();
            var userId = Helper.Helper.DecodeToken(UserToken);
            BasketItem isExist = _context.BasketItems.Include(p => p.Product).FirstOrDefault(b => b.ProductId == dbProduct.Id && b.AppUserId == userId);
            if (isExist != null)
            {
                if (isExist.Count < isExist.Product.StockCount)
                {
                    isExist.Count++;
                    isExist.Sum = isExist.Count * isExist.Price;

                    await _context.SaveChangesAsync();

                    List<BasketItem> basketItems = _context.BasketItems.Where(b => b.AppUserId == userId && b.ProductId == id).ToList();

                    List<BasketItem> dbBasketItems = _context.BasketItems.Where(b => b.AppUserId == userId).ToList();


                    foreach (var item in dbBasketItems)
                    {
                        total += item.Sum;
                    }
                    foreach (var item in basketItems)
                    {
                        obj = new
                        {
                            isExistCount = isExist.Count,
                            isExistSum = isExist.Sum,
                            ProductCount = isExist.Product.StockCount,
                            sum = item.Sum

                        };
                        frontBaskets.Add(obj);
                    }
                }
            }




            return Ok(new {total, frontBaskets});
        }

        [HttpPut("minus/{id}")]
        [Authorize]
        public async Task<IActionResult> Minus(int? id)
        {
            double total = 0;
            Product dbProduct = _context.Products.Include(p => p.Photos).FirstOrDefault(p => p.Id == id);
            List<object> frontBaskets = new List<object>();

            var obj = new object();

            string UserToken = HttpContext.Request.Headers["Authorization"].ToString();
            var userId = Helper.Helper.DecodeToken(UserToken);

            BasketItem isExist = _context.BasketItems.Include(p => p.Product).FirstOrDefault(b => b.ProductId == dbProduct.Id && b.AppUserId == userId);
            if (isExist != null)
            {
                if (isExist.Count > 1)
                {
                    isExist.Count--;
                    isExist.Sum = isExist.Count * isExist.Price;
                    await _context.SaveChangesAsync();
                    List<BasketItem> basketItems = _context.BasketItems.Where(b => b.AppUserId == userId && b.ProductId == id).ToList();

                    List<BasketItem> dbBasketItems = _context.BasketItems.Where(b => b.AppUserId == userId).ToList();


                    foreach (var item in dbBasketItems)
                    {
                        total += item.Sum;
                    }
                    foreach (var item in basketItems)
                    {

                        obj = new
                        {
                            isExistCount = isExist.Count,
                            isExistSum = isExist.Sum,
                            ProductCount = isExist.Product.StockCount,
                            sum = item.Sum

                        };
                        frontBaskets.Add(obj);
                    }

                }
            }
            return Ok(new { total, frontBaskets });

        }
        [HttpDelete("{id}")]
        [Authorize]

        public async Task<IActionResult> Remove(int? id)
        {
  
                double total = 0;
                Product dbProduct = _context.Products.Include(p => p.Photos).FirstOrDefault(p => p.Id == id);
                string UserToken = HttpContext.Request.Headers["Authorization"].ToString();
                var userId = Helper.Helper.DecodeToken(UserToken);
                BasketItem isExist = _context.BasketItems.Include(p => p.Product).FirstOrDefault(b => b.ProductId == dbProduct.Id && b.AppUserId == userId);
                if (isExist != null)
                {
                    _context.BasketItems.Remove(isExist);

                }
                await _context.SaveChangesAsync();
                List<BasketItem> basketItems = _context.BasketItems.Where(b => b.AppUserId == userId).ToList();
              
                foreach (var item in basketItems)
                {
                    total += item.Sum;
                  
                }
              
                return Ok(new { total});

        }

    }
}
