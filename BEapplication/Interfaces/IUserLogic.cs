using BEapplication.Models;

namespace BEapplication.Interfaces
{
    public interface IUserLogic
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        Task Register(RequestNewUser newUser);

        /// <summary>
        /// Logs in a user and returns a JWT token if successful.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string?> Login(UserLoginModel model);
    }
}
