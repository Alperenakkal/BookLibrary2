using BookLibary.Api.Models;
 namespace BookLibary.Api.Repositories{
 public interface IBookRepository<T> where T: class
     {

         Task<T> GetByIdAsync(string id);
         Task<T> InsertOneAsync(Book book);
         Task<GetManyResult<Book>> GetAllAsync(string name);
         GetOneResult<Book> DeleteById(string id);
    } 
 }