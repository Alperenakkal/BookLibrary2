using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;

namespace BookLibary.Api.Services.AuthServices.BorrowServices
{
    public class BorrowService : IBorrowService
    {
        private readonly IUserRepository<User> _userRepository;
        private readonly IBookRepository<Book> _bookRepository;

        private readonly IRepository<User> _repository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;



        public BorrowService(IUserRepository<User> userRepository, IRepository<User> repository, IHttpContextAccessor contextAccessor, IBookRepository<Book> bookRepository, IMemoryCache memoryCache)

        {
            _repository = repository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _memoryCache = memoryCache;
            _bookRepository = bookRepository;

        }
        public async Task<List<Book>> GetByNameAsync(string name)
        {
            User user = await _userRepository.GetByNameAsync(name);
            var bookList = new List<Book>();
            var borrowBooksList = user.BorrowBooks.ToList();
            foreach (var book in borrowBooksList)
            {
                var book1 = await _bookRepository.GetByIdAsync(book.ToString());
                if (book1 != null)
                {
                    var bookResponse = new Book
                    {
                        Id = book1.Id,
                        BookName = book1.BookName,
                        Author = book1.Author,
                        Publisher = book1.Publisher,
                        IsAvailable = false
                    };
                    bookList.Add(bookResponse);
                }
            }
           
            return bookList;
        }
        public async Task<User> GetByIdAsync(string id)
        {
            if (ObjectId.TryParse(id, out ObjectId objectId))
            {
                return await _userRepository.GetUserById(objectId);
            }
            throw new ArgumentException("Geçersiz ID formatı");
        }

        public async Task<GetOneResult<User>> UpdateUserAsync(string id, User user)
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
        public async Task<User> RemoveBookAsync(string userId, BarrowBookIdDto bookIdR)
        {
            ObjectId id = new ObjectId(userId);
            ObjectId bookId = new ObjectId(bookIdR.Id);
            var user = await _userRepository.RemoveBookFromUserAsync(userId, bookId);
            
            
            if(user == null)
            {
                throw new Exception("Kullanıcı bulunamadı");
            }
            return user;
            ;
        }
        public async Task AddBorrowedBookAsync(BarrowBookIdDto bookId,string userId)
        {
            
            var user = await GetByIdAsync(userId); // Kullanıcıyı ID'ye göre buluyoruz

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            // Kitap ID'sini ObjectId'ye dönüştürüyoruz
            ObjectId bookIdR;
            try
            {
                bookIdR = new ObjectId(bookId.Id);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Geçersiz kitap ID'si");
            }

            if (!user.BorrowBooks.Contains(bookIdR))
            {
               
                var borrowBooksList = user.BorrowBooks.ToList();
                borrowBooksList.Add(bookIdR);


                var userResponse = new User
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Password = user.Password,
                    BorrowBooks = borrowBooksList
                };
                

           
                var updateResult = await _userRepository.UpdateUserAsync(user.Id, userResponse);

                if (updateResult==null)
                {
                    throw new Exception("Kullanıcı güncellenemedi");
                }
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromDays(30));
                    await RemoveBookAsync(userId, bookId);
                });
            }
        }

            public async Task<bool> IsBookAvailableAsync(BarrowBookIdDto bookIdR){
            
            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            var redisToken = _memoryCache.Get("Bearer").ToString();
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(redisToken);
             
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var user = await GetByIdAsync(userId);
            ObjectId bookIdr = new ObjectId(bookIdR.Id);
            
            if (user.BorrowBooks.Contains(bookIdr))
            {
                throw new Exception("Kitap önceden ödünç alınmış");
            }
            return true;
        }
    }
}   

