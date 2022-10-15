using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TelloWebApi.Data;
using TelloWebApi.Models;

namespace TelloWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FagController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FagController(AppDbContext context)
        {
            _context = context;

        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Fag> fags = _context.Fags.ToList();
            return Ok(fags);
        }
    }
}
