using BookLibary.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;
namespace BookLibary.Api.Repositories{
 public interface IBookRepository<T> where T: class
     {

        Task<T> GetByIdAsync(string _id);
        Task<T> GetByNameAsync(string name);
        Task<T> InsertOneAsync(Book book);
         Task<GetManyResult<Book>> GetAllAsync();
         Task<GetOneResult<Book>> DeleteByNameAsync(string bookName);
        Task<Book> FindBookByNameAsync(string bookName);
        Task<Book> UpdateBookAsync(ObjectId id, Book updatedBook);
      


    }
}