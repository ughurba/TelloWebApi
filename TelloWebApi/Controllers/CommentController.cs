using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TelloWebApi.Data;
using TelloWebApi.Dtos.CommentDtos;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {

        private readonly AppDbContext _context;
        

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create(CommentCreateDto commentCreateDto)
        {
            DateTime dateTime = DateTime.Now;
            Comment comment = new Comment()
            {
                CreateTime = dateTime,
                AppUserId = commentCreateDto.AppUserId,
                Content = commentCreateDto.Content,
                ProductId = commentCreateDto.ProductId

            };
            

            _context.Add(comment);
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet("{id}")]
        public IActionResult GetAll(int id)
        {
            List<Comment> comments = _context.Comments.Include(c =>c.AppUser).Where(c => c.ProductId == id).ToList();

            return Ok(comments);
        }
    }
}
