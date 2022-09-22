using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TelloWebApi.Data;
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
        [HttpGet("search")]
        public IActionResult SearchProduct(string search)
        {

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

    }
}
