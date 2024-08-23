
using BookLibary.Api.Data.Context;
using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using MongoDB.Driver;
using System.Runtime.InteropServices;

namespace BookLibary.Api.Repositories
{
    public class CommentRepository : ICommentRepository<Comments>
    {
        private readonly IMongoCollection<Comments> _model;
        private readonly MongoDbContext _context;

        public CommentRepository(MongoDbContext context)
        {
            _context = context;
            _model = context.GetCollection<Comments>("Comments");
        }


        public async Task<Comments> InsertOneAsync(Comments comment)
        {
            await _model.InsertOneAsync(comment);
            return comment;
        }
        public async Task<List<Comments>> GetCommentByBookName(string bookName)
        {
            try
            {
                var filter = Builders<Comments>.Filter.Eq(x => x.BookName, bookName);
                return await _model.Find(filter).ToListAsync();
            }
            catch (Exception)
            {

                throw new Exception("GetCommentByBookNameRepository[CommentRepository 27] Fetch operation failed");
            }
        }
        public async Task<Comments> GetCommentByBookUserName(string userName)
        {
            try
            {
                var filter = Builders<Comments>.Filter.Eq(x => x.UserName, userName);
                return await _model.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw new Exception("GetCommentByBookNameRepository[CommentRepository 27] Fetch operation failed");
            }
        }
    }
}
