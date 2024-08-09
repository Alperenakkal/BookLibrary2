using BookLibary.Api.Data.Context;
using BookLibary.Api.Dtos.UserDto;
using BookLibary.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookLibary.Api.Repositories
{
    public class LoginRepository : IUserRepository<User>
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<User> _model;  

        public LoginRepository(MongoDbContext context)
        {
            _context = context;
            _model = context.GetCollection<User>("Users");
        }

       

        public async Task<User> GetByNameAsync(string name)
        {
            var filter = Builders<User>.Filter.Eq(x => x.UserName, name);
            return await _model.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserById(object _id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq("_id", _id);
                return await _model.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw new Exception("Id getirme işlemi başarısız");
            }
        }



        public async Task<User> UpdateUserAsync(object id, User entity)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq("_id", new ObjectId(id.ToString()));
                var update = Builders<User>.Update
          .Set(u => u.FullName, entity.FullName)
          .Set(u => u.UserName, entity.UserName)
          .Set(u => u.Email, entity.Email)
            .Set(u => u.gender, entity.gender)
           .Set(u => u.avatarUrl, entity.avatarUrl);
      


                var result = await _model.UpdateOneAsync(filter, update);

                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    return entity;
                }
                else
                {
                    throw new Exception("User not updated or not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Update Error", ex);
            }
        }


        public async Task<User> RemoveBookFromUserAsync(object userId, ObjectId bookId)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq("_id", new ObjectId(userId.ToString()));
                var update = Builders<User>.Update.Pull(
                    u => u.BorrowBooks,
                    bookId
                );

                var result = await _model.UpdateOneAsync(filter, update);

                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    // Kitap başarıyla silindi, güncellenmiş kullanıcıyı döndür
                    return await _model.Find(filter).FirstOrDefaultAsync();
                }
                else
                {
                    throw new Exception("Book not removed or user not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Update Error", ex);
            }
        }
    }

}

