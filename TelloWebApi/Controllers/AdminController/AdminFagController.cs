using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TelloWebApi.Data;
using TelloWebApi.Dtos.FagDtos;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers.AdminController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminFagController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminFagController(AppDbContext context)
        {
            _context = context;

        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            List<Fag> fags = _context.Fags.ToList();
            return Ok(fags);
        }
        [HttpPost]
        [Authorize]
        public IActionResult CreateFag(CreateFag createFag)
        {
            Fag newFag = new Fag
            {
                Key = createFag.Key,
                Value = createFag.Value
            };
            _context.Fags.Add(newFag);
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult RemoveFag(int id) {
            Fag fag = _context.Fags.FirstOrDefault(x => x.Id == id);
            _context.Remove(fag);
            _context.SaveChanges();
            return Ok();
        }

    }
}
