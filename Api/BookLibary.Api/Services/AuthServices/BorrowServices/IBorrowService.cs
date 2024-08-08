using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using MongoDB.Bson;

namespace BookLibary.Api.Services.AuthServices.BorrowServices
{
    public interface  IBorrowService
    {

        Task<List<Book>> GetBorrowBookAsync(string id);
        Task<User> GetByIdAsync(string id);
        Task<GetOneResult<User>> UpdateUserAsync(string id, User user);
        Task AddBorrowedBookAsync( BarrowBookIdDto bookId, string userId);
        Task<bool> IsBookAvailableAsync(BarrowBookIdDto bookIdR);
        Task<User> RemoveBookAsync(string userId, BarrowBookIdDto bookId);
        //Task RemoveBorrowedBookAsync( string bookId);
        Task AddtoReadoutBookAsync(BarrowBookIdDto bookId,string userId);

         Task<List<Book>> GetReadOutAsync(string id);
    }
}
