using Azure.Core;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookLibary.Api.Services.AuthServices.TokenHelperServices
{
    public class TokenHelperService : ITokenHelperService
    {

        public async Task<string> GetUserNameFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var userName = jsonToken?.Claims.FirstOrDefault(c => c.Type == "userName")?.Value;

            return await Task.FromResult(userName);


        }
    } 

}