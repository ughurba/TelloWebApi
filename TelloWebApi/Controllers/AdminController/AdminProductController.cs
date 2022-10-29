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
using TelloWebApi.Dtos.BrandDtos;
using TelloWebApi.Dtos.ProductDtos.ProductCreateDto;
using TelloWebApi.Dtos.ProductDtos.ProductReturnAdminDto;
using TelloWebApi.Dtos.ProductDtos.ProductReturnGetOneAdmin;
using TelloWebApi.Dtos.SpecificationsDto;
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
                    Path = "http://localhost:33033/img/" + item.SaveImage(_env, "img"),
                    IsMain = false
                };
                photos.Add(photo);
            }

            Photo isMainPhoto = new Photo
            {
                Path = "http://localhost:33033/img/" + productCreateDto.Photos.SaveImage(_env, "img"),
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
                Photos = photos
            };
            newProduct.ProductColors = new List<ProductColor>();
            newProduct.ProductStorages = new List<ProductStorage>();
            foreach (var item in productCreateDto.Colors)
            {
                ProductColor productColor = new ProductColor();
                Color color = new Color();
                color.Code = item;
                productColor.Colors = color;
                newProduct.ProductColors.Add(productColor);

            }
            if (productCreateDto.Storage != null)
            {
                foreach (var item in productCreateDto.Storage)
                {
                    ProductStorage productStorage = new ProductStorage();
                    Storage storage = new Storage();
                    storage.Value = item;
                    productStorage.Storage = storage;
                    newProduct.ProductStorages.Add(productStorage);

                }
            }


            _context.Add(newProduct);
            _context.SaveChanges();
            return StatusCode(201);
        }

        [HttpGet("{id}")]

        public IActionResult GetOne(int id)
        {
            Product product = _context.Products
                .Include(x => x.Photos)
                .Include(x => x.ProductColors)
                .ThenInclude(x => x.Colors)
                .Include(x => x.ProductStorages)
                .ThenInclude(x => x.Storage)
                .FirstOrDefault(x => x.Id == id);

            ProductReturnGetOneAdmin productReturnGetOneAdmin = new ProductReturnGetOneAdmin()
            {
                Id = product.Id,
                Description = product.Description,
                NewPrice = product.NewPrice,
                OldPrice = product.OldPrice,
                StockCount = product.StockCount,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                Title = product.Title,
                Colors = product.ProductColors.Select(x => new Color
                {
                    Id = x.ColorId,
                    Code = x.Colors.Code
                }).ToList(),
                Storages = product.ProductStorages.Select(x => new Storage
                {
                    Id = x.StorageId,
                    Value = x.Storage.Value
                }).ToList(),
                ChildPhotos = product.Photos.Where(x => !x.IsMain).ToList(),
                MainPhoto = product.Photos.FirstOrDefault(x => x.IsMain)


            };


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
                    Path = x.Path,
                    IsMain = x.IsMain,

                }).ToList(),
                brandName= p.Brand.Name,
                Description = p.Description,
                Title = p.Title,
                NewPrice = p.NewPrice,
                OldPrice = p.OldPrice,
                StockCount = p.StockCount,
                CategoryTitle = p.Category.Title
                

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
                string path = Path.Combine(_env.WebRootPath, "img", item.Path.Substring("http://localhost:33033/img/".Length));

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
        [Authorize]
        public async Task<IActionResult> Update([FromForm] ProductCreateDto product)
        {
            List<Photo> productImages = new List<Photo>();

            List<ProductColor> productColors = new List<ProductColor>();
            List<ProductStorage> productStorages = new List<ProductStorage>();

            Product dbProducts = _context.Products
                .Include(p => p.Photos)
                .Include(x => x.ProductColors)
                .ThenInclude(x => x.Colors)
                .Include(x => x.ProductStorages)
                .ThenInclude(x => x.Storage)
                .FirstOrDefault(c => c.Id == product.Id);
            Product productName = _context.Products.FirstOrDefault(p => p.Title.ToLower() == dbProducts.Title.ToLower());

            if (product.Photos != null)
            {


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
                foreach (var item in dbProducts.Photos)
                {
                    if (item.IsMain)
                    {
                        string path = Path.Combine(_env.WebRootPath, "img", item.Path.Substring("http://localhost:33033/img/".Length));
                        System.IO.File.Delete(path);
                    }

                }

                Photo photo = new Photo
                {
                    Path = "http://localhost:33033/img/" + product.Photos.SaveImage(_env, "img"),
                    IsMain = true
                };
                productImages.Add(photo);

            }


            if (product.ChildPhotos != null)
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
                foreach (var item in dbProducts.Photos)
                {
                    if (!item.IsMain)
                    {
                        string path = Path.Combine(_env.WebRootPath, "img", item.Path.Substring("http://localhost:33033/img/".Length));
                        System.IO.File.Delete(path);
                    }

                }
                foreach (var item in product.ChildPhotos)
                {

                    Photo photo = new Photo
                    {
                        Path = "http://localhost:33033/img/" + item.SaveImage(_env, "img"),
                        IsMain = false
                    };
                    productImages.Add(photo);
                }


            }


            if (productName != null)
            {
                if (productName.Title != dbProducts.Title)
                {
                    return BadRequest("bu adli product var ");
                }
            }

            if (product.ChildPhotos != null && product.Photos != null)
            {
                dbProducts.Photos = productImages;
            }

            dbProducts.Title = product.Title;
            dbProducts.NewPrice = product.NewPrice;
            dbProducts.OldPrice = product.OldPrice;
            dbProducts.Description = product.Description;
            dbProducts.StockCount = product.StockCount;

            foreach (var item in product.Colors)
            {
                ProductColor productColor = new ProductColor();
                Color color = new Color();
                color.Code = item;
                productColor.Colors = color;
                productColors.Add(productColor);

            }
            if (product.Storage != null)
            {
                foreach (var item in product.Storage)
                {
                    ProductStorage productStorage = new ProductStorage();
                    Storage storage = new Storage();
                    storage.Value = item;
                    productStorage.Storage = storage;
                    productStorages.Add(productStorage);

                }
            }

            dbProducts.ProductStorages = productStorages;
            dbProducts.ProductColors = productColors;
            await _context.SaveChangesAsync();
            return Ok();


        }
        [HttpPost("createSpec")]
        [Authorize]
        public IActionResult CreateSpecifications(CreateSpecifications createSpecifications)
        {
            ProductDetails productDetails = new ProductDetails
            {
                Name = createSpecifications.Key,
                Value = createSpecifications.Value,
                ProductId = createSpecifications.ProductId
            };
            _context.ProductDetails.Add(productDetails);
            _context.SaveChanges();
            return StatusCode(201);
        }
        [HttpGet("getSpecifications/{ProductId}")]
        [Authorize]
        public IActionResult GetAllSpecificationsProduct(int ProductId)
        {
            List<ProductDetails> productDetails = _context.ProductDetails.Where(x => x.ProductId == ProductId).ToList();
            return Ok(productDetails);
        }
        [HttpGet("{productId}/{specId}")]
        [Authorize]
        public IActionResult GetOneSpecification(int productId, int specId)
        {
            ProductDetails productDetail = _context.ProductDetails.FirstOrDefault(x => x.Id == specId && x.ProductId == productId);

            return Ok(productDetail);
        }
        [HttpPut("updateSpec")]
        [Authorize]
        public IActionResult UpdateSpecifications(UpdateSpecificationsDto updateSpecificationsDto)
        {
            ProductDetails productDetails = _context.ProductDetails.FirstOrDefault(x => x.ProductId == updateSpecificationsDto.ProductId && x.Id == updateSpecificationsDto.Id);
            productDetails.Name = updateSpecificationsDto.Key;
            productDetails.Value = updateSpecificationsDto.Value;
            _context.SaveChanges();
            return StatusCode(201);
        }
        [HttpDelete("removeSpec/{productId}/{specId}")]
        [Authorize]
        public IActionResult RemoveSpecifications(int productId, int specId)
        {
            ProductDetails productDetails = _context.ProductDetails.FirstOrDefault(x => x.Id == specId && x.ProductId == productId);
            _context.ProductDetails.Remove(productDetails);
            _context.SaveChanges();
            return StatusCode(200);
        }

        [HttpGet("brandProductsCount")]
        public IActionResult GetBrandsCount()
        {
            List<Brand> dbBrands = _context.Brand.ToList();
            List<ReturnBrandProductCountDto> brands = new List<ReturnBrandProductCountDto>();

            foreach (var item in dbBrands)
            {
                List<Product> products = _context.Products.Where(x => x.BrandId == item.Id).ToList();
                ReturnBrandProductCountDto newBrand = new ReturnBrandProductCountDto
                {
                    Name = item.Name,
                    Count = products.Count,
                };
                brands.Add(newBrand);
            }

            return Ok(brands);
        }
        [HttpGet("DateCountAddProduct")]
        [Authorize]
        public IActionResult DateAddProduct()
        {
            List<Product> products = _context.Products.ToList();
            List<Product> dbProducts = new List<Product>();
            int[] array = new int[12];


            foreach (var item in products)
            {
                var month = DateTime.Parse(item.CreatedDate.ToString()).Month;
                dbProducts = _context.Products.Where(x=>x.CreatedDate.Value.Month == month && !x.isDeleted).ToList();
                array[month - 1] = dbProducts.Count();
           
            }
            return Ok(array);
        }
    }

}
