using BookLibary.Api.Data.Context;
using BookLibary.Api.Dtos.UserDto;
using BookLibary.Api.Models;
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

        public async Task<User> GetUserByIdAsync(object _id)
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

        public async Task<User> UpdateUserAsync(User entity,Object _id)
        {
            

            try
            {
                var filter = Builders<User>.Filter.Eq("_id", _id);
                var updatedUser = await _model.ReplaceOneAsync(filter, entity);
                return entity;

            }
            catch (Exception)
            {

                throw new Exception("Update Error");
            }
        }
    }
}
