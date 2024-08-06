using BookLibary.Api.Models;
using BookLibary.Api.Repositories;

namespace BookLibary.Api.Services.AuthServices.TokenHelperServices
{
    public interface ITokenHelperService
    {
        Task<string> GetIdFromToken();

    }
}
