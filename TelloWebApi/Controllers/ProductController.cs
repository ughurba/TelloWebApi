using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TelloWebApi.Data;
using TelloWebApi.Dtos.PaginationDtos;
using TelloWebApi.Dtos.ProductDtos.ProductReturnDto;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = _context.Products.Include(p => p.Photos).Include(p => p.Category).Where(p => !p.isDeleted).ToList();
            List<ProductReturnDto> productReturnDtos = new List<ProductReturnDto>();
            foreach (var item in products)
            {
                ProductReturnDto productReturnDto = new ProductReturnDto();

                productReturnDto.Id = item.Id;
                productReturnDto.Title = item.Title;
                productReturnDto.Description = item.Description;
                productReturnDto.NewPrice = item.NewPrice;
                productReturnDto.OldPrice = item.OldPrice;
                productReturnDto.CategoryTitle = item.Category.Title;
                productReturnDto.inStock = item.inStock;

                foreach (var photo in item.Photos)
                {
                    productReturnDto.PhotoPath = photo.Path;
                    productReturnDto.isMainPhoto = photo.IsMain;
                };

                productReturnDtos.Add(productReturnDto);
            }

            return Ok(productReturnDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            Product product = _context.Products.Include(p => p.Photos)
                .Include(p => p.ProductColors)
                .ThenInclude(p => p.Colors)
                .Include(p => p.ProductStorages)
                .ThenInclude(p => p.Storage)
                .Include(p => p.Ratings)
                .Include(p => p.ProductDetails)
                .Include(p => p.Favorits)
                .Include(p => p.Comments)
                .Include(p => p.BasketItems)
                .ThenInclude(c => c.AppUser)
                .FirstOrDefault(p => p.Id == id && !p.isDeleted);

            return Ok(product);
        }

        [HttpGet("relatedProducts/{cateId}")]
        public IActionResult GetAllRelatedProduct(int cateId)
        {
            IQueryable<PaginationReturnDto> query = _context.Products
               .Include(p => p.Photos)
               .Include(p => p.Brand)
               .Include(p => p.Category)
               .Include(p => p.ProductColors)
               .ThenInclude(p => p.Colors)
               .Include(p => p.ProductDetails)
               .Where(x => x.CategoryId == cateId && !x.isDeleted )
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
            var result = query.ToList();
            return Ok(result);
        }
        [HttpGet("bestSelling")]
        public IActionResult GetBestSellingProduct()
        {
            IQueryable<ProductReturnDto> query = _context.Ratings
              .Include(r => r.Product)
                .ThenInclude(p => p.Photos)
                .Include(r => r.Product)
                .ThenInclude(p => p.Category)

                .Where(r => r.Avarge > 1 && !r.Product.isDeleted)
              .Select(x => new ProductReturnDto
              {
                  Id = x.ProductId,
                  Title = x.Product.Title,
                  Description = x.Product.Description,
                  NewPrice = x.Product.NewPrice,
                  OldPrice = x.Product.OldPrice,
                  CategoryTitle = x.Product.Category.Title,
                  inStock = x.Product.inStock,
                  Photos = x.Product.Photos.Select(x => new Photo
                  {
                      Id = x.ProductId,
                      Path = x.Path,
                      IsMain = x.IsMain

                  }).ToList()
              });
            var result = query.ToList();

            return Ok(result);
        }

        [HttpGet("newArrival")]
        public IActionResult GetNewArrivalProduct()
        {
            var from = DateTime.UtcNow.AddDays(-20);
            IQueryable<ProductReturnDto> query = _context.Products
                 .Include(p => p.Category)
                .Where(p => p.CreatedDate >= from && !p.isDeleted)
              .Select(x => new ProductReturnDto
              {
                  Id = x.Id,
                  Title = x.Title,
                  Description = x.Description,
                  NewPrice = x.NewPrice,
                  OldPrice = x.OldPrice,
                  CategoryTitle = x.Category.Title,
                  inStock = x.inStock,
                  Photos = x.Photos.Select(x => new Photo
                  {
                      Id = x.ProductId,
                      Path = x.Path,
                      IsMain = x.IsMain

                  }).ToList()
              });
            var result = query.ToList();

            return Ok(result);


        }


    }
}
