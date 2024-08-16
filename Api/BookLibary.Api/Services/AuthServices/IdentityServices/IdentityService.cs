using BookLibary.Api.Models;

namespace BookLibary.Api.Services.AuthServices.IdentityServices
{
    public class IdentityService : IIdentityService
    {
        public async Task<User> LoginByUserNameAndEmailQuery(User? userGetByUserName, User? userGetByEmail)
        {
            User user = new User();
            if (userGetByUserName != null) 
            {
                user=userGetByUserName;
            }
            if(userGetByEmail != null)
            {
               user=userGetByEmail;
            }
            return user;
           
        }
    }
}
