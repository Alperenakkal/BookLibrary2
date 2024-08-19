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
            User user = await _userRepository.GetByNameAsync(userName);

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            var bookList = new List<Book>();

            foreach (var readOutBook in user.ReadOutBooks)
            {
                var book = await _bookRepository.GetByNameAsync(readOutBook.BookName);

                if (book != null)
                {
                    var bookResponse = new Book
                    {
                        Id = book.Id,
                        BookName = book.BookName,
                        Author = book.Author,
                        Publisher = book.Publisher,
                        Category = book.Category,
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
            var user = await _userRepository.GetByNameAsync(userName);
            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı Bulunamadı");
            }

            var bookName = bookDto.bookName;
            var book = await _bookRepository.FindBookByNameAsync(bookName);

            if (book == null)
            {
                throw new KeyNotFoundException("Kitap Bulunamadı");
            }

            // Check if the book is already in the user's ReadOutBooks list and if BorrowCount has reached 3
            var readOutBook = user.ReadOutBooks.FirstOrDefault(b => string.Equals(b.BookName, bookName, StringComparison.OrdinalIgnoreCase));
            if (readOutBook != null && readOutBook.BorrowCount >= 3)
            {
                throw new InvalidOperationException("Bu kitabı artık ödünç alamazsınız, maksimum ödünç alma sayısına ulaştınız.");
            }

            // Check if the user has already borrowed 3 books
            if (user.BorrowBooks.Count >= 3)
            {
                throw new InvalidOperationException("Daha Fazla Kitap Ödünç Alamazsınız. Lütfen Kitap İade Edin.");
            }

            var borrowedBook = user.BorrowBooks.FirstOrDefault(b => string.Equals(b.BookName, bookName, StringComparison.OrdinalIgnoreCase));

            if (borrowedBook == null)
            {
                borrowedBook = new BorrowedBook
                {
                    BookName = bookName,
                    IsAvailable = false
                };
                user.BorrowBooks.Add(borrowedBook);

                // Decrease the book stock
                book.Stock -= 1;

                // Mark book as unavailable if out of stock
                if (book.Stock <= 0)
                {
                    book.IsAvailable = false;
                }

                var updateUserTask = await _userRepository.UpdateUserAsync(user.Id, user);
                if (updateUserTask == null)
                {
                    throw new Exception("Kullanıcı Güncellenemedi");
                }

                var updateBookTask = await _bookRepository.UpdateBookAsync(book.Id, book);
                if (updateBookTask == null)
                {
                    throw new Exception("Kitap Güncellenemedi");
                }

                // Schedule automatic return of the book after 30 days
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromDays(30));
                    await RemoveBookAsync(bookDto, userName);
                });
            }
            else
            {
                throw new InvalidOperationException("Kitap zaten kullanıcı tarafından ödünç alınmış.");
            }
        }





        //public async Task AddtoReadoutBookAsync(BorrowBookByNameDto bookDto, string userName)
        //{
        //    // Fetch user by username
        //    User user = await _userRepository.GetByNameAsync(userName);

        //    if (user == null)
        //    {
        //        throw new KeyNotFoundException("Kullanıcı bulunamadı.");
        //    }

        //    // Extract book name from DTO
        //    var bookName = bookDto.bookName;

        //    // Check if the book is in the user's borrowed books list
        //    var borrowedBook = user.BorrowBooks.FirstOrDefault(b => string.Equals(b.BookName, bookName, StringComparison.OrdinalIgnoreCase));

        //    if (borrowedBook != null)
        //    {
        //        // Remove the book from borrowed list
        //        user.BorrowBooks.Remove(borrowedBook);

        //        // Check if the book is already in the ReadOutBooks list
        //        var readOutBook = user.ReadOutBooks.FirstOrDefault(b => string.Equals(b.BookName, bookName, StringComparison.OrdinalIgnoreCase));

        //        if (readOutBook != null)
        //        {
        //            // Increment the borrow count for existing read-out book
        //            readOutBook.BorrowCount++;
        //        }
        //        else
        //        {
        //            // Add new book to ReadOutBooks list
        //            user.ReadOutBooks.Add(new ReadoutBook
        //            {
        //                BookName = bookName,
        //                BorrowCount = 1
        //            });
        //        }

        //        // Update book details
        //        var book = await _bookRepository.FindBookByNameAsync(bookName);
        //        if (book != null)
        //        {
        //            // Increase book stock and mark as available
        //            book.Stock += 1;
        //            if (!book.IsAvailable)
        //            {
        //                book.IsAvailable = true;
        //            }

        //            try
        //            {
        //                // Update user information
        //                var updateUserResult = await _userRepository.UpdateUserAsync(user.Id, user);
        //                if (updateUserResult == null)
        //                {
        //                    throw new Exception("Kullanıcı güncellenemedi");
        //                }

        //                // Update book information
        //                var bookUpdateResult = await _bookRepository.UpdateBookAsync(book.Id, book);
        //                if (bookUpdateResult == null)
        //                {
        //                    throw new Exception("Kitap güncellenemedi");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("Güncelleme işlemi başarısız", ex);
        //            }
        //        }
        //        else
        //        {
        //            throw new KeyNotFoundException("Böyle bir kitap bulunamadı.");
        //        }
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException("Kitap kullanıcı tarafından ödünç alınmamış.");
        //    }
        //}
        //eski hali
        public async Task AddtoReadoutBookAsync(BorrowBookByNameDto bookDto, string userName)
        {
            // Fetch user by username
            User user = await _userRepository.GetByNameAsync(userName);

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            // Extract book name from DTO
            var bookName = bookDto.bookName;

            // Check if the book is in the user's borrowed books list
            var borrowedBook = user.BorrowBooks.FirstOrDefault(b => string.Equals(b.BookName, bookName, StringComparison.OrdinalIgnoreCase));

            if (borrowedBook != null)
            {
                // Remove the book from borrowed list
                user.BorrowBooks.Remove(borrowedBook);

                // Check if the book is already in the ReadOutBooks list
                var readOutBook = user.ReadOutBooks.FirstOrDefault(b => string.Equals(b.BookName, bookName, StringComparison.OrdinalIgnoreCase));

                if (readOutBook != null)
                {
                    // If BorrowCount is 3, throw an exception to prevent further borrowing
                    if (readOutBook.BorrowCount >= 3)
                    {
                        throw new InvalidOperationException("Bu kitapı artık ödünç alamazsınız, maksimum ödünç alma sayısına ulaştınız.");
                    }

                    // Increment the borrow count
                    readOutBook.BorrowCount++;
                }
                else
                {
                    user.ReadOutBooks.Add(new ReadoutBook
                    {
                        BookName = bookName,
                        BorrowCount = 1
                    });
                }

                var book = await _bookRepository.FindBookByNameAsync(bookName);
                if (book != null)
                {
                    book.Stock += 1;
                    if (!book.IsAvailable)
                    {
                        book.IsAvailable = true;
                    }

                    try
                    {
                        var updateUserResult = await _userRepository.UpdateUserAsync(user.Id, user);
                        if (updateUserResult == null)
                        {
                            throw new Exception("Kullanıcı güncellenemedi");
                        }

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
 
        
    
 
