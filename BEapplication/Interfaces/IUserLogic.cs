using BEapplication.Models;

namespace BEapplication.Interfaces
{
    public interface IUserLogic
    {
        public Task AddUser(RequestNewUser newUser);

        public Task<User?> GetUser(UserLoginModel userLoginModel);
    }
}
