using Azure.Core;
using BookLibary.Api.Dtos.UserDto;
using BookLibary.Api.Models;
using BookLibary.Api.Models.Response.UserResponse;
using BookLibary.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace BookLibary.Api.Services.AuthServices.UpdateServices
{
    public class UpdateService : IUpdateService
    {
        private readonly IUserRepository<User> _repository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;


        public UpdateService(IUserRepository<User> repository, IHttpContextAccessor contextAccessor,IMemoryCache memoryCache)
        {
            _repository = repository;
            _contextAccessor = contextAccessor;
            _memoryCache = memoryCache;
        }





        public async Task<UpdateUserDto> UpdateUserAsync(User user)
        {
            UpdateUserDto dto = new UpdateUserDto();

            var token = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            var redisToken = _memoryCache.Get("Bearer").ToString();

          
            if (string.IsNullOrEmpty(redisToken))
            {
                throw new UnauthorizedAccessException("Token bulunamadı");
            }
            

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(redisToken);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User ID token'dan alınamadı");
            }

            if (string.IsNullOrEmpty(user.UserName) && string.IsNullOrEmpty(user.Password))
            {
                throw new NullReferenceException(nameof(user));
            }

            try
            {
                // ID'yi kullanarak kullanıcıyı güncelleme
                await _repository.UpdateUserAsync(userId, user);

                dto.UserName = user.UserName;
                dto.FullName = user.FullName;
                dto.Email = user.Email;
                if (!string.IsNullOrEmpty(user.Password))
                {
                    dto.Password = "Şifre başarılı bir şekilde değiştirildi";
                }
                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception("Güncelleme işlemi başarısız", ex);
            }
        }



    }
}