using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelloWebApi.Data;
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
                return BadRequest("bu adli category var");

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
                    return BadRequest("bu adli category var");
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
    }
}
