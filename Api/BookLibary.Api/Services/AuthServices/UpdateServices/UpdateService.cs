using Azure.Core;
using BookLibary.Api.Dtos.UserDto;
using BookLibary.Api.Models;
using BookLibary.Api.Models.Request.UserRequest;
using BookLibary.Api.Models.Response.UserResponse;
using BookLibary.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BookLibary.Api.Services.AuthServices.UpdateServices
{
   public class UpdateService : IUpdateService
{
    private readonly IUserRepository<User> _repository;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IMemoryCache _memoryCache;


        public UpdateService(IUserRepository<User> repository, IHttpContextAccessor contextAccessor, IMemoryCache memoryCache)
    {
        _repository = repository;
        _contextAccessor = contextAccessor;
        _memoryCache = memoryCache;
    }
    public async Task<UpdateUserDto> UpdatePassword(string name, updatePasswordRequest password)
    {
            try
            {
                var user = await _repository.UpdatePassword(name, password);
                UpdateUserDto userDto = new UpdateUserDto
                {
                    Email = user.Email,
                    FullName= user.FullName,
                    UserName = user.UserName,
                    Gender =user.gender,
                };
                return userDto;
            }
            catch (Exception ex)
            {

                throw new Exception("Kullanici sifre islemi basarisiz",ex);
            }
    }

    public async Task<UpdateUserDto> UpdateUserAsync(string userId,UpdateUserDto model)
    {
        UpdateUserDto dto = new UpdateUserDto();
            SHA1 sha = new SHA1CryptoServiceProvider();



          
            if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("User ID token'dan alınamadı");
        }

  
            var url = "";
            if (model.Gender == GenderType.Female)
            {
                url = $"https://avatar.iran.liara.run/public/girl/?username={model.UserName}";

            }
            else if (model.Gender == GenderType.other)
            {
                url = $"https://avatar.iran.liara.run/?username={model.UserName}";
            }
            else
            {
                url = $"https://avatar.iran.liara.run/public/boy/?username={model.UserName}";

            }

            try
        {
           
                var user = new User
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Email = model.Email,
          
                    gender =model.Gender,
                    avatarUrl=url,
                    IsAdmin = false,

                };
                await _repository.UpdateUserAsync(userId, user);

            dto.UserName = user.UserName;
            dto.FullName = user.FullName;
            dto.Email = user.Email;
          
            return dto;
        }
        catch (Exception ex)
        {
            throw new Exception("Güncelleme işlemi başarısız", ex);
        }
        }
    }
}
