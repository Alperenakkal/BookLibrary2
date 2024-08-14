using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using MongoDB.Bson;

namespace BookLibary.Api.Services.AuthServices.BorrowServices
{
    public interface  IBorrowService
    {

        Task<List<Book>> GetBorrowBookAsync(string userName);
        Task<User> GetByIdAsync(string id);
        Task<GetOneResult<User>> UpdateUserAsync(string id, User user);
        Task AddBorrowedBookAsync(BorrowBookByNameDto bookDto, string userName);
        Task<bool> IsBookAvailableAsync(BorrowBookByNameDto bookDto, string userName);
        Task<User> RemoveBookAsync(BorrowBookByNameDto bookDto, string userName);
        //Task RemoveBorrowedBookAsync( string bookId);
        Task AddtoReadoutBookAsync(BorrowBookByNameDto bookDto,string userName);

         Task<List<Book>> GetReadOutAsync(string userName);
    }
}
