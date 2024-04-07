using BEapplication.DBContexts;
using BEapplication.Interfaces;
using BEapplication.Models;
using Microsoft.EntityFrameworkCore;

namespace BEapplication.RequestHandlers
{
    public class UserLogic : IUserLogic
    {
        private ApplicationContext _context;
        
        public UserLogic(ApplicationContext context)
        {
            _context = context;
        }
        public async Task AddUser(RequestNewUser newUser)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = newUser.UserName,
                Email = newUser.Email,
                Password = newUser.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUser(UserLoginModel userLoginModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userLoginModel.Email);

            if (user == null)
            {
                return null;
            }

            if(user.Password != userLoginModel.Password)
            {
                return null;
            }

            return user;
        }
    }
}
