using BookLibary.Api.Dtos.UserDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;

namespace BookLibary.Api.Services.AuthServices.UpdateServices
{
	public interface IUpdateService
	{

		Task<UpdateUserDto> UpdateUserAsync(string userId, User user);
      
    }
}
