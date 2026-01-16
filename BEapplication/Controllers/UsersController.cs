using BEapplication.Interfaces;
using BEapplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BEapplication.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserLogic _userLogic;

        public UsersController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RequestNewUser model)
        {
            await _userLogic.Register(model);
            return Ok("User creat.");
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            var token = await _userLogic.Login(model);

            if (token == null)
                return Unauthorized("Email sau parola greșite");

            return Ok(new
            {
                token = token
            });
        }
    }
}
