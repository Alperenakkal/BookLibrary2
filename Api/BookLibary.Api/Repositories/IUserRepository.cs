using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Models.Request.UserRequest;
using MongoDB.Bson;

namespace BookLibary.Api.Repositories
{
    public interface IUserRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByNameAsync(string UserName);
        Task<TEntity> GetByEmailAsync(string email);
        Task<TEntity> UpdatePassword(string name ,updatePasswordRequest password);
        Task<TEntity> UpdateUserAsync(object id, User entity);
        Task<TEntity> UpdateUserBookAsync(object id, User entity);
        Task<TEntity> GetUserById(Object  _id);
        Task<User> RemoveBookFromUserAsync(BorrowBookByNameDto bookDto, string userName);

    }
}
