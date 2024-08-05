using Azure.Core;
using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Services.AuthServices;
using BookLibary.Api.Services.AuthServices.BorrowServices;
using BookLibary.Api.Services.AuthServices.TokenHelperServices;
using BookLibary.Api.Services.AuthServices.UpdateServices;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;

namespace BookLibary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowBookController : Controller
    {
        private readonly IBorrowService _borrowService;
        private readonly ITokenHelperService _tokenHelperService;
        private readonly IUpdateService _updateService;



        public BorrowBookController(IBorrowService borrowService, ITokenHelperService tokenHelperService, IUpdateService updateService)
        {
            _borrowService = borrowService;
            _tokenHelperService = tokenHelperService;
           
        }


        [HttpGet]
        public async Task<IActionResult> GetByNameAsync(string name)
        {
            User user = await _borrowService.GetByNameAsync(name);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> AddBorrowedBookAsync([FromBody] BorrowBookDto model)
        {
            var user = new User
            {
                BorrowBooks = [model.Id]
            };
               await _borrowService.AddBorrowedBookAsync(user);

            return Ok("Kitap başarıyla ödünç alındı.");
        }


    }
}
