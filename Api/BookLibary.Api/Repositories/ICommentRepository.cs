using BookLibary.Api.Models;
using MongoDB.Bson;

namespace BookLibary.Api.Repositories
{
    public interface ICommentRepository<T>where T: class
    {
        Task<T> InsertOneAsync(Comments comment);
        Task<List<T>> GetCommentByBookName(string bookName);
        Task<T> GetCommentByBookUserName(string userName);
    }
}
