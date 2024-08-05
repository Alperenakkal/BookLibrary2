using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using MongoDB.Bson;

namespace BookLibary.Api.Services.AuthServices.BorrowServices
{
    public interface  IBorrowService
    {

        Task<User> GetByNameAsync(string name);
        Task<GetOneResult<User>> UpdateUserAsync(string id, User user);
        Task AddBorrowedBookAsync( BarrowBookIdDto bookId);
        Task<bool> IsBookAvailableAsync(BarrowBookIdDto bookIdR);


    }
}
