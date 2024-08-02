namespace BookLibary.Api.Repositories
{
    public interface IUserRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByNameAsync(string userName);
        Task<TEntity> UpdateUserAsync(Object _id,TEntity entity);
        Task<TEntity> GetUserById(Object  _id);
    }
}
