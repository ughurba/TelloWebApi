using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelloWebApi.Data;
using TelloWebApi.Dtos.PaginationDtos;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> dbCategories = _context.Categories.ToList();
            return Ok(dbCategories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id, int page = 1, int size = 6)
        {
            IQueryable<PaginationReturnDto> query = _context.Products
                .Include(p => p.Photos)
                .Include(p => p.Category)
                .Include(p => p.ProductColors)
                .ThenInclude(p => p.Colors)
                .Include(p=>p.ProductDetails)
                .Where(p => !p.isDeleted && p.CategoryId == id)
                .Select(x => new PaginationReturnDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    CategoryId = x.CategoryId,
                    Description = x.Description,
                    NewPrice = x.NewPrice,
                    OldPrice = x.OldPrice,
                    CategoryTitle = x.Category.Title,
                    
                    inStock = x.inStock,
                  
                    Colors = x.ProductColors.Select(x => new Color
                    {
                        Id = x.Colors.Id,
                        Code = x.Colors.Code,
                    }).ToList(),
                   
                    Photos = x.Photos.Select(x => new Photo
                    {
                        Path = x.Path,
                        IsMain = x.IsMain

                    }).ToList()
                });


            var totalCount = await query.CountAsync();
            
            var result = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            

            return Ok(new { totalCount, result });

        }

        [HttpGet("brand")]
        public IActionResult GetBrand()
        {
            List<Brand> brands = _context.Brand.ToList();
            return Ok(brands);
        }


    }
}
