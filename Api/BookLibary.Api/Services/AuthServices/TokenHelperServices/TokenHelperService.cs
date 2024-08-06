using Azure.Core;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookLibary.Api.Services.AuthServices.TokenHelperServices
{
    public class TokenHelperService : ITokenHelperService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;

        public TokenHelperService(IHttpContextAccessor contextAccessor, IMemoryCache memoryCache)
        {
           
            _contextAccessor = contextAccessor;
            _memoryCache = memoryCache;
         

        }
        public async Task<string> GetIdFromToken()
        {
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
                throw new InvalidOperationException("Geçersiz kullanıcı kimliği");
            }


            return userId;


        }
    } 

}