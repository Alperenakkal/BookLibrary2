using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using BookLibary.Api.Services.AuthServices.UpdateServices;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

        private readonly IUpdateService _updateService;



        public BorrowService(IUserRepository<User> userRepository, IRepository<User> repository, IHttpContextAccessor contextAccessor, IBookRepository<Book> bookRepository, IMemoryCache memoryCache, IUpdateService updateService)

        {
            _repository = repository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            _memoryCache = memoryCache;
            _bookRepository = bookRepository;
            _updateService = updateService;

        }
        public async Task<List<Book>> GetBorrowBookAsync(string userName)
        {
          //  ObjectId userId = new ObjectId(id);
            User user = await _userRepository.GetByNameAsync(userName);
            var bookList = new List<Book>();
            var borrowBooksList = user.BorrowBooks.ToList();
            foreach (var book in borrowBooksList)
            {
                var book1 = await _bookRepository.GetByNameAsync(book.ToString());
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
        public async Task<List<Book>> GetReadOutAsync(string userName)
        {
           // ObjectId userId = new ObjectId(id);
            User user = await _userRepository.GetByNameAsync(userName);
            var bookList2 = new List<Book>();
            var readOutBookList = user.ReadOutBooks.ToList();
            foreach (var book in readOutBookList)
            {
                var book1 = await _bookRepository.GetByNameAsync(book);
                if (book1 != null)
                {
                    var bookResponse2 = new Book
                    {
                        Id = book1.Id,
                        BookName = book1.BookName,
                        Author = book1.Author,
                        Publisher = book1.Publisher,
                        IsAvailable = false
                    };
                    bookList2.Add(bookResponse2);
                }
            }

            return bookList2;

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
        public async Task<User> RemoveBookAsync(BorrowBookByNameDto bookDto, string userName)
        {
            //    ObjectId id = new ObjectId(userId);
            //   ObjectId bookId = new ObjectId(bookIdR.Id);
      
            var user = await _userRepository.RemoveBookFromUserAsync(bookDto,userName);


            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı");
            }
            return user;
            ;
        }
        public async Task AddBorrowedBookAsync(BorrowBookByNameDto bookDto, string userName)
        {
            var user = await _userRepository.GetByNameAsync(userName); // Kullanıcıyı kullanıcı adına göre buluyoruz

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            // Kitap adını alıyoruz
            var bookName = bookDto.bookName;

            // Kitabın veritabanında olup olmadığını kontrol ediyoruz
            var book = await _bookRepository.FindBookByNameAsync(bookName);
            if (book == null)
            {
                throw new KeyNotFoundException("Böyle bir kitap bulunamadı.");
            }

            // Eğer kitap okunanlar listesinde ise, uyarı mesajı hazırla
            bool alreadyRead = user.ReadOutBooks.Contains(bookName);
            if (alreadyRead)
            {
                // Uyarıyı döndürecek ama işlem devam edecek
                Console.WriteLine("Bu kitabı zaten okudunuz.");
            }

            // Kitabı BorrowBooks listesine ekle
            if (!user.BorrowBooks.Contains(bookName))
            {
                var borrowBooksList = user.BorrowBooks.ToList();
                borrowBooksList.Add(bookName);

                var userResponse = new User
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    gender = user.gender,
                    avatarUrl = user.avatarUrl,
                    ReadOutBooks = user.ReadOutBooks,
                    Password = user.Password,
                    BorrowBooks = borrowBooksList
                };

                var updateResult = await _userRepository.UpdateUserAsync(user.Id, userResponse);

                if (updateResult == null)
                {
                    throw new Exception("Kullanıcı güncellenemedi");
                }

                // 30 gün sonra kitabı otomatik olarak geri vermek için bir görev başlatıyoruz
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromDays(30));
                    await RemoveBookAsync(bookDto, userName);
                });
            }

            // Eğer kitap zaten okunanlar listesinde ise bir uyarı mesajı döndür
            if (alreadyRead)
            {
                throw new InvalidOperationException("Bu kitabı zaten okudunuz.");
            }
        }




        public async Task<bool> IsBookAvailableAsync(BorrowBookByNameDto bookDto, string userName)
        {

            //  var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            //// var redisToken = _memoryCache.Get("Bearer").ToString();
            // var tokenHandler = new JwtSecurityTokenHandler();
            // var jwtToken = tokenHandler.ReadJwtToken(redisToken);

            // var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
           // ObjectId userId = new ObjectId(id);
            User user = await _userRepository.GetByNameAsync(userName);

            //     var user = await GetByIdAsync(id);
          // ObjectId bookIdr = new ObjectId(bookIdR.Id);
          var bookName = bookDto.bookName;
            if (user.BorrowBooks.Contains(bookName))
            {
                throw new Exception("Kitap önceden ödünç alınmış");
            }
            return true;
        }
        public async Task AddtoReadoutBookAsync(BorrowBookByNameDto bookDto, string userName)
        {
            // Kullanıcıyı kullanıcı adıyla bul
            User user = await _userRepository.GetByNameAsync(userName);

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı."); // User not found
            }

            // Kitap adını DTO'dan al
            var bookName = bookDto.bookName;

            // Kullanıcının ödünç aldığı kitaplar arasında bu kitap var mı kontrol et
            if (user.BorrowBooks.Contains(bookName))
            {
                var borrowBooksList = user.BorrowBooks.ToList();
                var readOutBooksList = user.ReadOutBooks.ToList();

                // Kitabı ödünç alınanlardan çıkar
                borrowBooksList.Remove(bookName);

                // Eğer kitap okunanlar listesinde değilse, ekleyelim
                if (!readOutBooksList.Contains(bookName))
                {
                    readOutBooksList.Add(bookName);
                }
                else
                {
                    Console.WriteLine("Kitap zaten okunanlar listesinde mevcut.");
                }

                // Kullanıcı nesnesini güncelle
                user.BorrowBooks = borrowBooksList;
                user.ReadOutBooks = readOutBooksList;

                try
                {
                    var updateResult = await _userRepository.UpdateUserAsync(user.Id, user);
                    Console.WriteLine("Okunan kitap listesi güncellendi.");
                }
                catch (Exception ex)
                {
                    throw new Exception("Güncelleme işlemi başarısız", ex);
                }
            }
            else
            {
                throw new InvalidOperationException("Kitap kullanıcı tarafından ödünç alınmamış.");
            }
        }


    }
}
 
        
    
 
