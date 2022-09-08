using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TelloWebApi.Data;
using TelloWebApi.Dtos.ProductDtos.ProductCreateDto;
using TelloWebApi.Dtos.ProductDtos.ProductReturnDto;
using TelloWebApi.Extentions;
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
            List<Product> products = _context.Products.Where(p => !p.isDeleted).ToList();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            Product product = _context.Products.Include(p => p.Photos)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductStorages)
                .Include(p => p.Ratings)
                .Include(p => p.ProductDetails)
                .Include(p => p.Favorits)
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == id && !p.isDeleted);
            return Ok(product);
        }

        [HttpGet("bestSelling")]
        public IActionResult GetBestSellingProduct()
        {
            List<Rating> dbRatings = _context.Ratings
                .Include(r => r.Product)
                .ThenInclude(p => p.Photos)
                .Where(r => r.Avarge > 5 && !r.Product.isDeleted)
                .ToList();
            ProductReturnDto productReturnDto = new ProductReturnDto();
            List<ProductReturnDto> productReturnDtos = new List<ProductReturnDto>();
            foreach (var item in dbRatings)
            {
                productReturnDto.Photos = item.Product.Photos;
                productReturnDto.NewPrice = item.Product.NewPrice;
                productReturnDto.OldPrice = item.Product.OldPrice;
                productReturnDto.Title = item.Product.Title;
                productReturnDtos.Add(productReturnDto);
            }

            return Ok(productReturnDtos);
        }

        [HttpGet("newArrival")]
        public IActionResult GetNewArrivalProduct()
        {
            var from = DateTime.UtcNow.AddDays(-1);
            List<Product> products = _context.Products.Where(p => p.CreatedDate >= from).ToList();
            return Ok(products);

        }


        [HttpPost("createProduct")]
        public IActionResult Create([FromForm] ProductCreateDto productCreateDto)
        {


            foreach (var item in productCreateDto.ChildPhotos)
            {

                if (item == null)
                {

                    return BadRequest("Bosqoyma");
                }
                if (!item.IsImage())
                {

                    return BadRequest("only Photo");

                }
                if (item.ValidSize(200))
                {
                    return BadRequest("olcu uygun deyil");
                }


            }

            if (productCreateDto.Photos == null)
            {

                return BadRequest("Bosqoyma");
            }
            if (!productCreateDto.Photos.IsImage())
            {

                return BadRequest("only Photo");

            }
            if (productCreateDto.Photos.ValidSize(200))
            {
                return BadRequest("olcu uygun deyil");
            }

            List<Photo> photos = new List<Photo>();

            foreach (var item in productCreateDto.ChildPhotos)
            {

                Photo photo = new Photo
                {
                    Path = item.SaveImage(_env, "img"),
                    IsMain = false
                };
                photos.Add(photo);
            }

            Photo isMainPhoto = new Photo
            {
                Path = productCreateDto.Photos.SaveImage(_env, "img"),
                IsMain = true,
            };
            photos.Add(isMainPhoto);



            Product newProduct = new Product
            {
                Title = productCreateDto.Title,
                Description = productCreateDto.Description,
                NewPrice = productCreateDto.NewPrice,
                OldPrice = productCreateDto.OldPrice,
                inStock = true,
                BrandId = productCreateDto.BrandId,
                CategoryId = productCreateDto.CategoryId,
                StockCount = productCreateDto.StockCount,
                CreatedDate = DateTime.Now,
                Photos = photos,
                ProductColors = new List<ProductColor>
                {
                    new ProductColor
                    {
                        Colors =  new Color
                         {
                             Code = productCreateDto.Color,

                         }
                    }

                },
                ProductStorages = new List<ProductStorage>
                {
                    new ProductStorage
                    {
                        Storage =  new Storage
                         {
                             Value = productCreateDto.Storage,

                         }
                    }

                }

            };
         
            _context.Add(newProduct);
            _context.SaveChanges();
            return StatusCode(201);
        }

        [HttpGet("brandAndCategoryIds")]
        public IActionResult GetBrandAndCategoryId()
        {
            List<Brand> dbBrands = _context.Brand.Where(b => !b.IsDeleted).ToList();

            List<Category> dbCategory = _context.Categories.ToList();

            var obj = new
            {
                Brand = dbBrands,
                Category = dbCategory
            };
            return Ok(obj);
        }

        [HttpPut]
        public IActionResult Update(Product product)
        {
            return StatusCode(200);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Product p = _context.Products.FirstOrDefault(p => p.Id == id);
            if (p == null)
            {
                return NotFound();
            }
            p.isDeleted = true;
            _context.SaveChanges();
            return StatusCode(200);
        }
    }
}
