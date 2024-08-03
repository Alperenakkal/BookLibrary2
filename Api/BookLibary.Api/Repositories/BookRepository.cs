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

                // Debugging purposes
                if (data == null)
                {
                    result.Message = "No data found.";
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
                result.Message = $"An error occurred while retrieving data: {ex.Message}";
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

        public Task<Book> GetByIdAsync(string id)
        {
             var filter = Builders<Book>.Filter.Eq(x => x.BookName, id);
             return _collection.Find(filter).FirstOrDefaultAsync();
        }

      
    }
 } 