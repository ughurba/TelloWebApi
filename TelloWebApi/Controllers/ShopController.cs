using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TelloWebApi.Data;
using TelloWebApi.Dtos.FavoriteDtos;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IWebHostEnvironment _env;

        public ShopController(AppDbContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }



        [HttpPost("search")]
        public IActionResult SearchProduct(string search)
        {
            if (search == null)
            {
                return BadRequest();
            }
            List<Product> result = _context.Products
                .Include(p => p.Photos)
                .Include(p => p.Category)
                .Include(p => p.ProductColors)
                .ThenInclude(p => p.Colors)
                .Include(p => p.ProductDetails)
                .OrderBy(p => p.Id)
                .Where(p => p.Title.ToLower()
                .Contains(search.ToLower()))
                .ToList();

            return Ok(new { result });
        }

        [HttpPost("favorite")]
        [Authorize]
        public IActionResult Favorites(CreateFavoriteDto createFavoriteDto)
        {
            string UserToken = HttpContext.Request.Headers["Authorization"].ToString();
            var userId = Helper.Helper.DecodeToken(UserToken);
            
                var favorites = _context.Favorits.FirstOrDefault(x=>createFavoriteDto.favId == x.Id && userId == x.AppUserId);
            if (favorites == null)
            {
                Favorit favorit = new Favorit()
                    {
                        AppUserId = userId,
                        ProductId = createFavoriteDto.ProductId

                    };
                    _context.Add(favorit);
                    _context.SaveChanges();
                    return StatusCode(201);
            }


            Favorit dbFavorit = _context.Favorits.FirstOrDefault(f => f.ProductId == createFavoriteDto.ProductId && f.AppUserId == userId);
            _context.Remove(dbFavorit);
            _context.SaveChanges();
            return StatusCode(200);
        }


        [HttpGet("favoriteGet")]
        [Authorize]
        public IActionResult GetAllFavorite()
        {
            string UserToken = HttpContext.Request.Headers["Authorization"].ToString();
            var userId = Helper.Helper.DecodeToken(UserToken); 
            List<Favorit> favorits = _context.Favorits
                .Include(x=>x.Product)
                .ThenInclude(x=>x.Photos)
                .Where(x => x.AppUserId == userId).ToList();
            List<Product> products = new List<Product>();
            foreach (var item in favorits)
            {
                products.Add(item.Product);
            }
            return Ok(products); 
        }

    }
}
