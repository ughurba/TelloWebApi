using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TelloWebApi.Data;
using TelloWebApi.Dtos.ProductDtos.ProductCreateDto;
using TelloWebApi.Dtos.ProductDtos.ProductReturnAdminDto;
using TelloWebApi.Dtos.ProductDtos.ProductReturnGetOneAdmin;
using TelloWebApi.Extentions;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProductController : ControllerBase
    {

        private IWebHostEnvironment _env;
        private readonly AppDbContext _context;

        public AdminProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
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

        [HttpGet("{id}")]

        public IActionResult GetOne(int id)
        {
            ProductReturnGetOneAdmin productReturnGetOneAdmin = _context.Products.Select(x=>new ProductReturnGetOneAdmin
            {
                Id = id,
                Description = x.Description,
                NewPrice=x.NewPrice,
                OldPrice=x.OldPrice,
                StockCount=x.StockCount,
                BrandId=x.BrandId,
                CategoryId = x.CategoryId,
                Title = x.Title,
            }).FirstOrDefault(x => x.Id == id);

            return Ok(productReturnGetOneAdmin);
        }

        [HttpGet("getAll")]
        [Authorize]
        public IActionResult GetAll()
        {
            IQueryable<ProductReturnAdminDto> query = _context.Products.Where(x => !x.isDeleted).Select(p => new ProductReturnAdminDto
            {
                Id = p.Id,
                Photos = p.Photos.Select(x => new Photo
                {
                    //$"http://localhost:33033/img/
                    Path = x.Path,
                    IsMain = x.IsMain,

                }).ToList(),
                Description = p.Description,
                Title = p.Title,
                NewPrice = p.NewPrice,
                OldPrice = p.OldPrice,
                StockCount = p.StockCount
            });

            
            var result = query.ToList();
            return Ok(result);

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
         


            List<BasketItem> basketItems = _context.BasketItems.Include(b => b.Product).Where(b => b.ProductId == id).ToList();
            Product dbproduct = _context.Products.Include(p => p.Photos).FirstOrDefault(p => p.Id == id);
            foreach (var item in dbproduct.Photos)
            {
                string path = Path.Combine(_env.WebRootPath, "img", item.Path);

                if (dbproduct == null)
                    Helper.Helper.DeleteImage(path);
            }

            foreach (var item in basketItems)
            {
                item.Product.isDeleted = true;
            }
            dbproduct.isDeleted = true;
            _context.SaveChanges();

            return StatusCode(200);
        }

        [HttpPut("updateProduct")]
        public async Task<IActionResult> Update(ProductCreateDto product)
        {
            List<Photo> productImages = new List<Photo>();
          

            Product products = _context.Products.Include(p => p.Photos).FirstOrDefault(c => c.Id == product.Id);
            Product productName = _context.Products.FirstOrDefault(c => c.Title.ToLower() == product.Title.ToLower());

            if (product.Photos != null)
            {
                foreach (var item in product.ChildPhotos)
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

                if (product.Photos == null)
                {

                    return BadRequest("Bosqoyma");
                }
                if (!product.Photos.IsImage())
                {

                    return BadRequest("only Photo");

                }
                if (product.Photos.ValidSize(200))
                {
                    return BadRequest("olcu uygun deyil");
                }
            


            }
            if (productName != null)
            {
                if (productName.Title != products.Title)
                {
                    return BadRequest("bu adli product var ");
                }
            }


            products.Title = product.Title;
            products.NewPrice = product.NewPrice;
            products.OldPrice = product.OldPrice;
            products.Description = product.Description;
            products.StockCount = product.StockCount;
            products.inStock = product.inStock;


            await _context.SaveChangesAsync();
            return Ok();


        }
    }
}
