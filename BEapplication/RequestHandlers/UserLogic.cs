using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BEapplication.DBContexts;
using BEapplication.Interfaces;
using BEapplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BEapplication.RequestHandlers
{
    public class UserLogic : IUserLogic
    {
        private readonly ApplicationContext _context;

        public UserLogic(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Inheritance
        /// </summary>
        public async Task Register(RequestNewUser newUser)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);
            if (dbUser != null)
                throw new Exception("Email already used.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = newUser.Name,
                Email = newUser.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Inheritance
        /// </summary>
        public async Task<string?> Login(UserLoginModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                throw new Exception("No account found with this email.");
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return null;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("THIS_IS_A_VERY_LONG_SUPER_SECRET_JWT_KEY_2026!")
            );


            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
