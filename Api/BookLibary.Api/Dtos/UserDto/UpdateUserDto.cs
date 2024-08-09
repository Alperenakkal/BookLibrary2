using BookLibary.Api.Models;
using MongoDB.Bson;

namespace BookLibary.Api.Dtos.UserDto
{
    public class UpdateUserDto
    {
       
        public string UserName { get; set; }

        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public GenderType Gender { get; set; }
    }
}

