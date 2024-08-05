using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using BookLibary.Api.Services.AuthServices.TokenServices;
using BookLibary.Api.Services.AuthServices.UpdateServices;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;

namespace BookLibary.Api.Services.AuthServices.BorrowServices
{
    public class BorrowService : IBorrowService
    {
        private readonly IUserRepository<User> _userRepository;

        private readonly IRepository<User> _repository;
        private readonly IHttpContextAccessor _contextAccessor;
        //private readonly IMongoCollection<User> _model;


        public BorrowService(IUserRepository<User> userRepository,IRepository<User> repository, IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
          //  _model = model;
        }

        public async Task<User> GetByNameAsync(string id)
        {
            if (ObjectId.TryParse(id, out ObjectId objectId))
            {
                return await _userRepository.GetUserById(objectId);
            }
            throw new ArgumentException("Geçersiz ID formatı");
        }

        public async Task<GetOneResult<User>> UpdateUserAsync(ObjectId id, User user)
        {
            BorrowBookDto dto = new BorrowBookDto();
            try
            {
                // ID'yi kullanarak kullanıcıyı güncelleme
              //  await _repository.UpdateUserAsync(id, user);

                dto.Id = user.Id;
                
            }
            catch (Exception ex)
            {
                throw new Exception("Güncelleme işlemi başarısız", ex);
            }
            return await _repository.ReplaceOneAsync(user, id.ToString());
        }

        public async Task AddBorrowedBookAsync( ObjectId bookId)
        {
            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();


            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Token bulunamadı");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException(userId);
            }
            
            var user = await GetByNameAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            if (!user.BorrowBooks.Contains(bookId))
            {
                user.BorrowBooks.Add(bookId);
            }

            await UpdateUserAsync(bookId,user);
        }
    }
}
