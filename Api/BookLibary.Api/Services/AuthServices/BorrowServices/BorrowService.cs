using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using BookLibary.Api.Services.AuthServices.TokenServices;
using BookLibary.Api.Services.AuthServices.UpdateServices;
using MongoDB.Bson;

namespace BookLibary.Api.Services.AuthServices.BorrowServices
{
    public class BorrowService : IBorrowService
    {
        private readonly IUserRepository<User> _userRepository;

        private readonly IRepository<User> _repository;

        public BorrowService(IUserRepository<User> userRepository,IRepository<User> repository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<User> GetByNameAsync(string name)
        {
            User user = await _userRepository.GetByNameAsync(name);
            return user;
        }

        public async Task<GetOneResult<User>> UpdateUserAsync(string name, User user)
        {
            return await _repository.ReplaceOneAsync(user, name);
        }

        public async Task AddBorrowedBookAsync(string name, ObjectId bookId)
        {
            var user = await GetByNameAsync(name);

            if (user != null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            if (!user.BorrowBooks.Contains(bookId))
            {
                user.BorrowBooks.Add(bookId);
            }

            await UpdateUserAsync(name, user);
        }
    }
}
