﻿using Microsoft.AspNetCore.Hosting;
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
        [HttpGet("getProductInShop")]
        public async Task<IActionResult> GetProduct(int id, [FromQuery] int?[] brandIds, int orderBy, double minPrice, double maxPrice,string userId, bool discount = false, bool allBrand = false, int page = 1, int size = 6)
        {

     
            IQueryable<PaginationReturnDto> query = _context.Products
                .Include(p => p.Photos)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(x=>x.Favorits)
                .Include(p => p.ProductColors)
                .ThenInclude(p => p.Colors)
                .Include(p => p.ProductDetails)
                .Where(p => !p.isDeleted
                            && (discount || allBrand ? true : p.CategoryId == id)
                            && p.NewPrice >= minPrice
                            && p.NewPrice <= maxPrice
                            && (brandIds.Length == 0 ? true : brandIds.Contains(p.BrandId))
                            && (discount ? p.OldPrice > 0 : true))
                .Select(x => new PaginationReturnDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    CategoryId = x.CategoryId,
                    Description = x.Description,
                    NewPrice = x.NewPrice,
                    OldPrice = x.OldPrice,
                    CategoryTitle = x.Category.Title,
                    Favorits = x.Favorits,
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

            query = orderBy == 0 ? query.OrderBy(p => p.NewPrice) : query.OrderByDescending(p => p.NewPrice);


            var result = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            List<int> productIdFavorite = new List<int>();
            if (userId != null)
            {
                List<Favorit> favorits = _context.Favorits.Where(x=>x.AppUserId == userId).ToList();
                foreach (var item in favorits)
                {
                    productIdFavorite.Add(item.ProductId);
                }
            }


            return Ok(new { totalCount, result, productIdFavorite });

        }


        [HttpGet("brand")]
        public IActionResult GetBrand()
        {
            List<Brand> brands = _context.Brand.ToList();
            return Ok(brands);
        }


    }
}
