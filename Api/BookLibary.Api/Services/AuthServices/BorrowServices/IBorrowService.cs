using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using MongoDB.Bson;

namespace BookLibary.Api.Services.AuthServices.BorrowServices
{
    public interface  IBorrowService
    {

        Task<User> GetByNameAsync(string name);
        Task<GetOneResult<User>> UpdateUserAsync(ObjectId id, User user);
        Task AddBorrowedBookAsync( ObjectId bookId);


    }
}
