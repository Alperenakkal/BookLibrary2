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
                        IsAvailable = book1.IsAvailable,
                        Stock = book1.Stock,
                        Category = book1.Category,
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
                        Category = book1.Category,
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
    // Find the user by username
    var user = await _userRepository.RemoveBookFromUserAsync(bookDto, userName);

    if (user == null)
    {
        throw new Exception("Kullanıcı bulunamadı");
    }

    // Get the book name from the DTO
    var bookName = bookDto.bookName;

    // Find the book in the repository
    var book = await _bookRepository.FindBookByNameAsync(bookName);
    if (book == null)
    {
        throw new KeyNotFoundException("Böyle bir kitap bulunamadı.");
    }

    // İade edildiğinde stoğu arttır
    book.Stock += 1;

    // önceden mevcut değilse stoğa geldiğinde IsAvailable ı artık true yap
    if (!book.IsAvailable && book.Stock > 0)
    {
        book.IsAvailable = true;
    }

    // Update the book in the repository
    var bookUpdateResult = await _bookRepository.UpdateBookAsync(book.Id, book);
    if (bookUpdateResult == null)
    {
        throw new Exception("Kitap güncellenemedi");
    }

    return user;
}

        public async Task AddBorrowedBookAsync(BorrowBookByNameDto bookDto, string userName)
        {
            var user = await _userRepository.GetByNameAsync(userName); // Find user by username

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            // kitap isimlerini Dto çekme
            var bookName = bookDto.bookName;

            // Veritabanında böyle bir kitap var mı Kontrolü
            var book = await _bookRepository.FindBookByNameAsync(bookName);
            if (book == null)
            {
                throw new KeyNotFoundException("Böyle bir kitap bulunamadı.");
            }

            // stok kontrol
            if (book.Stock <= 0)
            {
                book.IsAvailable = false;
                await _bookRepository.UpdateBookAsync(book.Id, book); 
                throw new InvalidOperationException("Bu kitap stokta kalmadı.");
            }

            //3'ten fazla Ödünç Kitap Alamamamızı sağlıyor...
            if (user.BorrowBooks.Count > 2)
            {
                throw new InvalidOperationException("Daha Fazla Ödünç Kitap Alamazsınız.Başka kitapları Ödünç Alabilmek İçin Lütfen Ödünç Listenizden kitap çıkarın.");

            }
            // Kitabın zaten ReadOutBooks listesinde olup olmadığını kontrol edin

            bool alreadyRead = user.ReadOutBooks.Contains(bookName);
            if (alreadyRead)
            {
                throw new InvalidOperationException("Bu kitabı zaten okudunuz.");
            }

            // Henüz orada değilse, kitabı Ödünç Kitaplar listesine ekleyin

            if (!user.BorrowBooks.Contains(bookName))
            {
                var borrowBooksList = user.BorrowBooks.ToList();
                borrowBooksList.Add(bookName);
                book.Stock -= 1;

                // Kitap kullanılabilirliğini yeni stok düzeyine göre güncelleyin

                if (book.Stock <= 0)
                {
                    book.IsAvailable = false;
                }
               
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
                var bookUpdateResult = await _bookRepository.UpdateBookAsync(book.Id, book);

                if (updateResult == null)
                {
                    throw new Exception("Kullanıcı güncellenemedi");
                }

                // Start a task to automatically return the book after 30 days
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromDays(30));
                    await RemoveBookAsync(bookDto, userName);
                });
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
            //userı kullanıcı adına göre bul
            User user = await _userRepository.GetByNameAsync(userName);

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı."); 
            }

            // Dto'dan kitap adını çek
            var bookName = bookDto.bookName;

            // ödünç alınanlarda mı diye Kontrol
            if (user.BorrowBooks.Contains(bookName))
            {
                var borrowBooksList = user.BorrowBooks.ToList();
                var readOutBooksList = user.ReadOutBooks.ToList();

                // Ödünç Listesinden Kaldır
                borrowBooksList.Remove(bookName);

                // Okunanlarda değilse okunanlara ekle
                if (!readOutBooksList.Contains(bookName))
                {
                    readOutBooksList.Add(bookName);
                }
                else
                {
                    Console.WriteLine("Kitap zaten okunanlar listesinde mevcut.");
                }

                // userın listesinin güncelle
                user.BorrowBooks = borrowBooksList;
                user.ReadOutBooks = readOutBooksList;

                try
                {
                    // user infosu güncelle
                    var updateResult = await _userRepository.UpdateUserAsync(user.Id, user);
                    Console.WriteLine("Okunan kitap listesi güncellendi.");

                    // stok ve mevcudiyeti güncelle
                    var book = await _bookRepository.FindBookByNameAsync(bookName);
                    if (book != null)
                    {
                        book.Stock += 1;

                        // önceden mevcut değilse ve iade edilince stok açıldıysa IsAvailable ı true döndür
                        if (!book.IsAvailable && book.Stock > 0)
                        {
                            book.IsAvailable = true;
                        }

                        var bookUpdateResult = await _bookRepository.UpdateBookAsync(book.Id, book);
                        if (bookUpdateResult == null)
                        {
                            throw new Exception("Kitap güncellenemedi");
                        }
                    }
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
 
        
    
 
