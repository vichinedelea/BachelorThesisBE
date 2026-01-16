using BEapplication.Models;

namespace BEapplication.Interfaces
{
    public interface IUserLogic
    {
        Task Register(RequestNewUser newUser);
        Task<string?> Login(UserLoginModel model);
    }
}
