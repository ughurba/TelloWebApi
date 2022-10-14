using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelloWebApi.Data;
using TelloWebApi.Dtos.BrandDtos;
using TelloWebApi.Dtos.CategoryDtos;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryAndBrandController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryAndBrandController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAllBrandAndCategory()
        {
            List<Category> categories = _context.Categories.ToList();
            List<Brand> brands = _context.Brand.ToList();
            return Ok(new { categories, brands });
        }
        [HttpPost("createCategory")]
        [Authorize]
        public async Task<IActionResult> Create(Category category)
        {

            bool existNameCategory = _context.Categories.Any(c => c.Title == category.Title);
            if (existNameCategory)
            {
                return BadRequest("Xəta baş verib:Eyni adlı kateqoriya artıq mövcuddur");

            }
            Category newCategory = new Category
            {
                Title = category.Title,
                isActive = category.isActive,
            };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();


            return Ok();
        }
        [HttpPut("categoryUpdate")]
        [Authorize]
        public IActionResult CategoryUpdate(CategoryUpdateDto categoryUpdateDto)
        {

            Category dbCategory = _context.Categories.FirstOrDefault(c => c.Id == categoryUpdateDto.Id);
            Category dbCategoryName = _context.Categories.FirstOrDefault(c => c.Title.ToLower() == categoryUpdateDto.Title.ToLower());
            if (dbCategoryName != null)
            {
                if (dbCategoryName.Title != dbCategory.Title)
                {
                    return BadRequest("Xəta baş verib:Eyni adlı kateqoriya artıq mövcuddur");
                }
            }
            dbCategory.Title = categoryUpdateDto.Title;
            dbCategory.isActive = categoryUpdateDto.isActive;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("removeCategory/{id}")]
        [Authorize]
        public IActionResult Delete(int? id)
        {
            Category dbCategory = _context.Categories.FirstOrDefault(c => c.Id == id);

            _context.Categories.Remove(dbCategory);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("createBrand")]
        [Authorize]
        public async Task<IActionResult> CreateBrand(Brand brand)
        {

            bool existNameBrand = _context.Brand.Any(c => c.Name == brand.Name);
            if (existNameBrand)
            {
                return BadRequest("Xəta baş verib:Eyni adlı brend artıq mövcuddur");

            }
            Brand newBrand = new Brand
            {
                Name = brand.Name,

            };
            await _context.Brand.AddAsync(newBrand);
            await _context.SaveChangesAsync();


            return Ok();
        }
        [HttpPut("brandUpdate")]
        [Authorize]
        public IActionResult brandUpdate(UpdateBrandDto updateBrandDto)
        {

            Brand dbBrand = _context.Brand.FirstOrDefault(c => c.Id == updateBrandDto.Id);
            Brand dbBrandName = _context.Brand.FirstOrDefault(c => c.Name.ToLower() == updateBrandDto.Name.ToLower());
            if (dbBrandName != null)
            {
                if (dbBrandName.Name != dbBrand.Name)
                {
                    return BadRequest("Xəta baş verib:Eyni adlı brend artıq mövcuddur");
                }
            }
            dbBrand.Name = updateBrandDto.Name;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("removeBrand/{id}")]
        [Authorize]
        public IActionResult DeleteBrand(int? id)
        {
            Brand dbBrand = _context.Brand.FirstOrDefault(c => c.Id == id);

            _context.Brand.Remove(dbBrand);
            _context.SaveChanges();
            return Ok();
        }






    }
}



