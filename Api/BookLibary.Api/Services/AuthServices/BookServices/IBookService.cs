
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;

namespace BookLibary.Api.Services.AuthServices.BookServices
{
    public interface IBookService
    {
        Task<GetManyResult<Book>> GetAllBooksAsync(string name);
        Task<Book> GetByIdAsync(string id);
        Task<Book> CreateBookAsync(Book book);
        // Task<GetOneResult<Book>> UpdateBookAsync(string id, Book book);
        GetOneResult<Book> DeleteBook(string id);

    }
}