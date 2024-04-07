using BEapplication.Models;

namespace BEapplication.Interfaces
{
    public interface IUserLogic
    {
        public Task AddUser(User user);

        public Task<User?> GetUser(UserLoginModel userLoginModel);
    }
}
