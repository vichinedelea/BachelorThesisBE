﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BEapplication.DBContexts;
using BEapplication.Models;
using BEapplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace BEapplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IUserLogic _userLogic;

        public UsersController(ApplicationContext context, IUserLogic userLogic)
        {
            _context = context;
            _userLogic = userLogic;
        }

        //POST: api/Users

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("/addUser")]
        [AllowAnonymous]
        [EnableCors("AllowLocalhost3000")]
        public async Task<IActionResult> AddUser(RequestNewUser newUser)
        {
            await _userLogic.AddUser(newUser);

            return Ok();
        }

        // Post: api/Users
        [HttpPost("/ckeckUser")]
        public async Task<ActionResult> CkeckUser(UserLoginModel userLoginModel)
        {
            var userExists = await _userLogic.CkeckUser(userLoginModel);

            if (userExists == false)
            {
                return NotFound(); // Returnați NotFound dacă utilizatorul nu este găsit
            }
            else
            {
                return Ok();
            }
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
