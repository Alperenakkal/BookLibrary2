using System.Collections;
using BookLibary.Api.Data.Context;
using BookLibary.Api.Models;
 using Microsoft.Extensions.Options;
 using MongoDB.Bson;
 using MongoDB.Driver;


 namespace BookLibary.Api.Repositories
 {
     public class BookRepository : IBookRepository<Book>
     {
         private readonly IMongoCollection<Book> _collection;
         private readonly MongoDbContext _context;

         //private readonly IMongoCollection<Book> _model;
       

         public BookRepository(MongoDbContext context,IOptions<MongoSettings> settings)
         {
             var client = new MongoClient(settings.Value.ConnectionString);
             var database = client.GetDatabase(settings.Value.Database);

             _context = context;
             //_model = context.GetCollection<Book>("Books");
             _collection = database.GetCollection<Book>("Books");  // (typeof(Book).Name)

         }
         
         public async Task<Book> InsertOneAsync(Book book)
         {
             await _collection.InsertOneAsync(book);
             return book;
         }
        public async Task<GetManyResult<Book>> GetAllAsync()
        {
            var result = new GetManyResult<Book>();
            try
            {
                var data = await _collection.Find(_ => true).ToListAsync();

                if (data == null)
                {
                    result.Message = "Kitap Bulunamadı";
                    result.Success = false;
                }
                else
                {
                    result.Result = data;
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Message = "Kitaplar Yüklenemedi";
                result.Success = false;
            }
            return result;
        }


        public async Task<GetOneResult<Book>> DeleteByNameAsync(string bookName)
        {
            var result = new GetOneResult<Book>();
            try
            {
                var filter = Builders<Book>.Filter.Eq(x => x.BookName, bookName);
                var data = await _collection.FindOneAndDeleteAsync(filter);
                result.Entity = data;
                result.Success = data != null;
            }
            catch (Exception ex)
            {
                result.Message = $"An error occurred while deleting the book: {ex.Message}";
                result.Success = false;
            }
            return result;
        }

        public async Task<Book> GetByIdAsync(string _id)
        {
            if (!ObjectId.TryParse(_id, out ObjectId objectId))
            {
                throw new ArgumentException("Geçersiz ID formatı");
            }

            try
            {
                var filter = Builders<Book>.Filter.Eq("_id", objectId);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Id getirme işlemi başarısız: " + ex.Message);
            }
        }
        public async Task<Book> GetByNameAsync(string name)
        {
            try
            {
                var filter = Builders<Book>.Filter.Eq(x => x.BookName, name);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw new Exception("Book Not found");
            }
        }
        public async Task<Book> FindBookByNameAsync(string bookName)
        {
            var filter = Builders<Book>.Filter.Eq(b => b.BookName, bookName);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Book> UpdateBookAsync(ObjectId id, Book updatedBook)
        {
            var filter = Builders<Book>.Filter.Eq(b => b.Id, id);
            var update = Builders<Book>.Update
                .Set(b => b.AverageRating, updatedBook. )
                .Set(b => b.TotalRating, updatedBook.TotalRating)
                .Set(b => b.RatingCount, updatedBook.RatingCount);

            var result = await _collection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged && result.ModifiedCount > 0 ? updatedBook : null;
        }



    }
} 