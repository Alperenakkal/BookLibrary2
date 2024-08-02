using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
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

        public GetOneResult<Book> DeleteBook(string id)
        {
            return  _bookRepository.DeleteById(id);
        }

        public async Task <GetManyResult<Book>> GetAllBooksAsync(string name)
        {
            return await _bookRepository.GetAllAsync(name);
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
