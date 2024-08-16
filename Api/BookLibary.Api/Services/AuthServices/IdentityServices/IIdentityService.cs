using BookLibary.Api.Models;

namespace BookLibary.Api.Services.AuthServices.IdentityServices
{
    public interface IIdentityService
    {
        Task<User> LoginByUserNameAndEmailQuery(User?userGetByUserName, User? userGetByEmail);
    }
}
