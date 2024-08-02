using Azure.Core;
using BookLibary.Api.Dtos.UserDto;
using BookLibary.Api.Models;
using BookLibary.Api.Models.Response.UserResponse;
using BookLibary.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace BookLibary.Api.Services.AuthServices.UpdateServices
{
    public class UpdateService : IUpdateService
    {
        private readonly IUserRepository<User> _repository;
        private readonly IHttpContextAccessor _contextAccessor;

        public UpdateService(IUserRepository<User> repository, IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _contextAccessor = contextAccessor;
        }





        public async Task<UpdateUserDto> UpdateUserAsync(User user)
        {
            UpdateUserDto dto = new UpdateUserDto();

           
            var token = await _contextAccessor.HttpContext.GetTokenAsync("AuthToken");

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Token bulunamadı");
            }

            
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

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