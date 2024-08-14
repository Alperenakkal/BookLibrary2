using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using MongoDB.Bson;

namespace BookLibary.Api.Repositories
{
    public interface IUserRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByNameAsync(string userName);
        Task<TEntity> GetByEmailAsync(string email);
        Task<User> UpdateUserAsync(object id, User entity);
        Task<TEntity> GetUserById(Object  _id);
        Task<User> RemoveBookFromUserAsync(BorrowBookByNameDto bookDto, string userName);
    }
}
