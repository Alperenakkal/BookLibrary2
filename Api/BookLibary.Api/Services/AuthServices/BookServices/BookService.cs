using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using MongoDB.Bson;
//using BookLibary.Api.Controllers;



namespace BookLibary.Api.Services.AuthServices.BookServices
{
    public class BookService:IBookService
    {
        private readonly IBookRepository<Book> _bookRepository;

        public BookService(IBookRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
             return await _bookRepository.InsertOneAsync(book);
        }

        public async Task<GetOneResult<Book>> DeleteBook(string bookName)
        {
            return await _bookRepository.DeleteByNameAsync(bookName);
        }

        public async Task <GetManyResult<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }
        public async Task<Book> GetByIdAsync(string id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        // public Task<GetOneResult<Book>> UpdateBookAsync(string id, Book book)
        // {
        //     throw new NotImplementedException();
        // }
    }
}
