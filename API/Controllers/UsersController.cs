using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly niktopler_getTogetherContext _context;

        public UsersController(niktopler_getTogetherContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Users>> GetAll()
        {
            return Ok(_context.Users);
        }
    }
}
