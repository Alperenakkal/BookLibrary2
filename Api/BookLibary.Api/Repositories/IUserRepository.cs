using BookLibary.Api.Models;

namespace BookLibary.Api.Repositories
{
    public interface IUserRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByNameAsync(string userName);
        Task<User> UpdateUserAsync(object id, User entity);
        Task<TEntity> GetUserById(Object  _id);
    }
}
