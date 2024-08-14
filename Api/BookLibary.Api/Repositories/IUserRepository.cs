using BookLibary.Api.Models;
using BookLibary.Api.Models.Request.UserRequest;
using MongoDB.Bson;

namespace BookLibary.Api.Repositories
{
    public interface IUserRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByNameAsync(string userName);
        Task<TEntity> GetByEmailAsync(string email);
        Task<TEntity> UpdatePassword(string name ,updatePasswordRequest password);
        Task<TEntity> UpdateUserAsync(object id, User entity);

        Task<TEntity> GetUserById(Object  _id);
        Task<TEntity> RemoveBookFromUserAsync(object userId, ObjectId bookId);
    }
}
