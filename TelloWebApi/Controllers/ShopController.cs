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

        [HttpGet("getAllBrands")]
        public IActionResult GetBrand()
        {
            List<Brand> brands = _context.Brand.ToList();
            return Ok();
        }

        [HttpGet("")]
        public IActionResult FilterBrand(int id)
        {
            List<Product> products = _context.Products.Where(p => p.BrandId == id).ToList();
            return Ok(products);
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
            if (createFavoriteDto.isFavorite)
            {
                Favorit favorit = new Favorit()
                {
                    AppUserId = userId,
                    PrdocutId = createFavoriteDto.ProductId
                    
                };
                _context.Add(favorit);
                _context.SaveChanges();
                return StatusCode(201);
            }
            Favorit dbFavorit = _context.Favorits.FirstOrDefault(f => f.PrdocutId == createFavoriteDto.ProductId && f.AppUserId == userId);
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
            List<Product> result = new List<Product>();
            List<Favorit> favorits = _context.Favorits.Where(x => x.AppUserId == userId).ToList();
            List<Product> dbProducts = _context.Products
                .Include(p => p.Photos)
                .Include(p => p.Category)
                .Include(p => p.ProductColors)
                .ThenInclude(p => p.Colors)
                .Include(p => p.ProductDetails).ToList();
            foreach (var product in dbProducts)
            {
                foreach (var item in favorits)
                {
                    if (item.PrdocutId == product.Id)
                    {
                        product.isFavorite = true;
                        result.Add(product);
                    }
                }
            }
            return Ok(new{result });
        }
      

    }
}
