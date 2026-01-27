using BEapplication.Interfaces;
using BEapplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                await _userLogic.Register(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            try
            {
                var token = await _userLogic.Login(model);

                if (token == null)
                    return Unauthorized("Wrong email or password");

                return Ok(new
                {
                    token = token
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
