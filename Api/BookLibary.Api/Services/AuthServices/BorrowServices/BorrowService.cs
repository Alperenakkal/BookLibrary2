﻿using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using BookLibary.Api.Services.AuthServices.TokenServices;
using BookLibary.Api.Services.AuthServices.UpdateServices;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;

namespace BookLibary.Api.Services.AuthServices.BorrowServices
{
    public class BorrowService : IBorrowService
    {
        private readonly IUserRepository<User> _userRepository;

        private readonly IRepository<User> _repository;
        private readonly IHttpContextAccessor _contextAccessor;
        //private readonly IMongoCollection<User> _model;


        public BorrowService(IUserRepository<User> userRepository, IRepository<User> repository, IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _userRepository = userRepository;
            _contextAccessor = contextAccessor;
            //  _model = model;
        }

        public async Task<User> GetByNameAsync(string id)
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

        public async Task AddBorrowedBookAsync(BarrowBookIdDto bookId)
        {
            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Token bulunamadı");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("Geçersiz kullanıcı kimliği");
            }

            var user = await GetByNameAsync(userId); // Kullanıcıyı ID'ye göre buluyoruz

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
            }
            if (user.BorrowBooks.Contains(bookIdR))
            {
                throw new Exception("Kitap önceden ödünç alınmış");
            }
        }

    }
}
