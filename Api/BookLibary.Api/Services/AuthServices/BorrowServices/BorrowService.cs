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
            // Kullanıcıyı kullanıcı adına göre bul
            User user = await _userRepository.GetByNameAsync(userName);

            // Kullanıcının ödünç aldığı kitaplar listesi
            var bookList = new List<Book>();
            var borrowBooksList = user.BorrowBooks.ToList();

            foreach (var borrowedBook in borrowBooksList)
            {
                var book = await _bookRepository.FindBookByNameAsync(borrowedBook.BookName);

                if (book != null)
                {
                    var bookResponse = new Book
                    {
                        Id = book.Id,
                        BookName = book.BookName,
                        Author = book.Author,
                        Publisher = book.Publisher,
                        // Sadece o kullanıcı için IsAvailable değerini set et
                        IsAvailable = borrowedBook.IsAvailable,
                        Stock = book.Stock,
                        Category = book.Category,
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
            // Kullanıcıyı bul
            var user = await _userRepository.GetByNameAsync(userName);

            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı");
            }

            // Kitap adını al
            var bookName = bookDto.bookName;

            // Kullanıcının ödünç aldığı kitaplar listesinden kitabı bul
            var borrowedBook = user.BorrowBooks.FirstOrDefault(b => string.Equals(b.BookName, bookName, StringComparison.OrdinalIgnoreCase));

            if (borrowedBook == null)
            {
                throw new InvalidOperationException("Bu kitap kullanıcı tarafından ödünç alınmamış.");
            }

            // Kitap adını kullanarak kitabı bul
            var book = await _bookRepository.FindBookByNameAsync(bookName);
            if (book == null)
            {
                throw new KeyNotFoundException("Böyle bir kitap bulunamadı.");
            }

            // Kitabı kullanıcıdan kaldır
            user.BorrowBooks.Remove(borrowedBook);

            // Kitap stokunu artır
            book.Stock += 1;

            // Kitap mevcut değilse IsAvailable durumunu true yap
            if (!book.IsAvailable)
            {
                book.IsAvailable = true;
            }

            try
            {
                // Kullanıcı bilgilerini güncelle
                var updateUserResult = await _userRepository.UpdateUserAsync(user.Id, user);
                if (updateUserResult == null)
                {
                    throw new Exception("Kullanıcı güncellenemedi");
                }

                // Kitap bilgilerini güncelle
                var bookUpdateResult = await _bookRepository.UpdateBookAsync(book.Id, book);
                if (bookUpdateResult == null)
                {
                    throw new Exception("Kitap güncellenemedi");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Güncelleme işlemi başarısız", ex);
            }

            return user;
        }




        public async Task AddBorrowedBookAsync(BorrowBookByNameDto bookDto, string userName)
        {
            var user = await _userRepository.GetByNameAsync(userName); // Kullanıcıyı kullanıcı adına göre bul
            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            var bookName = bookDto.bookName;
            var book = await _bookRepository.FindBookByNameAsync(bookName); // Kitabı isme göre bul

            if (book == null)
            {
                throw new KeyNotFoundException("Böyle bir kitap bulunamadı.");
            }

            if (user.BorrowBooks.Count > 2)
            {
                throw new InvalidOperationException("Daha Fazla Ödünç Kitap Alamazsınız. Başka kitapları ödünç alabilmek için lütfen ödünç listenizden kitap çıkarın.");
            }

            if (user.ReadOutBooks.Any(b => string.Equals(b, bookName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Bu kitabı zaten okudunuz.");
            }

            var borrowedBook = user.BorrowBooks.FirstOrDefault(b => string.Equals(b.BookName, bookName, StringComparison.OrdinalIgnoreCase));

            if (borrowedBook == null)
            {
                // Kitap henüz kullanıcı tarafından ödünç alınmadıysa, ekleyin ve IsAvailable durumunu false yapın
                borrowedBook = new BorrowedBook
                {
                    BookName = bookName,
                    IsAvailable = false // Bu kullanıcı için kitabı artık ödünç aldı
                };
                user.BorrowBooks.Add(borrowedBook);

                // Kitap stoğunu azalt
                book.Stock -= 1;

                // Eğer stok bitti ve kitap daha önce mevcutsa IsAvailable durumunu false yap
                if (book.Stock <= 0)
                {
                    book.IsAvailable = false;
                }

                var updateUserTask = await _userRepository.UpdateUserAsync(user.Id, user);

                if (updateUserTask == null)
                {
                    throw new Exception("Kullanıcı güncellenemedi");
                }

                var updateBookTask = await _bookRepository.UpdateBookAsync(book.Id, book);

                if (updateBookTask == null)
                {
                    throw new Exception("Kitap güncellenemedi");
                }

                // 30 gün sonra kitabı otomatik olarak iade etme işlemini başlat
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromDays(30));
                    await RemoveBookAsync(bookDto, userName);
                });
            }
            else
            {
                throw new InvalidOperationException("Bu kitabı zaten ödünç aldınız.");
            }
        }


        public async Task AddtoReadoutBookAsync(BorrowBookByNameDto bookDto, string userName)
        {
            // Kullanıcıyı kullanıcı adına göre bul
            User user = await _userRepository.GetByNameAsync(userName);

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            // DTO'dan kitap adını çek
            var bookName = bookDto.bookName;

            // Kullanıcının ödünç aldığı kitaplar arasında kitabın olup olmadığını kontrol et
            var borrowedBook = user.BorrowBooks.FirstOrDefault(b => string.Equals(b.BookName, bookName, StringComparison.OrdinalIgnoreCase));

            if (borrowedBook != null)
            {
                // Kitabı ödünç kitaplar listesinden çıkar
                user.BorrowBooks.Remove(borrowedBook);

                // Kitap daha önce okunanlar listesinde değilse, okunanlara ekle
                if (!user.ReadOutBooks.Contains(bookName))
                {
                    user.ReadOutBooks.Add(bookName);
                }
                else
                {
                    Console.WriteLine("Kitap zaten okunanlar listesinde mevcut.");
                }

                // Kitap bilgilerini güncelle
                var book = await _bookRepository.FindBookByNameAsync(bookName);
                if (book != null)
                {
                    // Kitap stoğunu artır ve mevcut değilse IsAvailable'ı true yap
                    book.Stock += 1;
                    if (!book.IsAvailable)
                    {
                        book.IsAvailable = true;
                    }

                    try
                    {
                        // Kullanıcı bilgilerini güncelle
                        var updateUserResult = await _userRepository.UpdateUserAsync(user.Id, user);
                        if (updateUserResult == null)
                        {
                            throw new Exception("Kullanıcı güncellenemedi");
                        }

                        // Kitap bilgilerini güncelle
                        var bookUpdateResult = await _bookRepository.UpdateBookAsync(book.Id, book);
                        if (bookUpdateResult == null)
                        {
                            throw new Exception("Kitap güncellenemedi");
                        }

                        Console.WriteLine("Okunan kitap listesi güncellendi.");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Güncelleme işlemi başarısız", ex);
                    }
                }
                else
                {
                    throw new KeyNotFoundException("Böyle bir kitap bulunamadı.");
                }
            }
            else
            {
                throw new InvalidOperationException("Kitap kullanıcı tarafından ödünç alınmamış.");
            }
        }





    }
}
 
        
    
 
