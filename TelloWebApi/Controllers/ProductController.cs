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
                .Where(p => p.CreatedDate >= from)
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
