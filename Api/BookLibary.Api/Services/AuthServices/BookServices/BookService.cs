using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace BookLibary.Api.Services.AuthServices.BookServices
{
    public class BookService : IBookService
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

        public async Task<GetManyResult<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Book> GetByIdAsync(string id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<Book> GetByNameAsync(string name)
        {
            return await _bookRepository.GetByNameAsync(name);
        }

        public async Task<RateBookResultDto> RateBookAsync(RateBookRequest request)
        {
            if (!ObjectId.TryParse(request.BookId, out ObjectId objectId))
            {
                return new RateBookResultDto { Success = false, AverageRating = 0, Message = "Geçersiz kitap ID'si" };
            }

            var book = await _bookRepository.GetByIdAsync(request.BookId);
            if (book == null)
            {
                return new RateBookResultDto { Success = false, AverageRating = 0, Message = "Kitap bulunamadı" };
            }

            book.RatingCount++;
            book.TotalRating += request.Rating;
            book.AverageRating = book.TotalRating / book.RatingCount;

            var updatedBook = await _bookRepository.UpdateBookAsync(objectId, book);

            if (updatedBook != null)
            {
                return new RateBookResultDto
                {
                    Success = true,
                    AverageRating = updatedBook.AverageRating
                };
            }

            return new RateBookResultDto { Success = false, AverageRating = 0, Message = "Kitap güncelleme işlemi başarısız" };
        }


    }

}
