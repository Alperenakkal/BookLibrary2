using Azure.Core;
using BookLibary.Api.Models;
using BookLibary.Api.Services.AuthServices;
using BookLibary.Api.Services.AuthServices.BorrowServices;
using BookLibary.Api.Services.AuthServices.TokenHelperServices;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BookLibary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowBookController : Controller
    {
        private readonly IBorrowService _borrowService;
        private readonly ITokenHelperService _tokenHelperService;

        public BorrowBookController(IBorrowService borrowService, ITokenHelperService tokenHelperService)
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
        public async Task<IActionResult> AddBorrowedBookAsync([FromBody] ObjectId Id)
        {

            //var userId = User.FindFirst("userId")?.Value;
            //var userName = await _tokenHelperService.GetUserNameFromToken()

            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token bulunamadı.");
            }

           // var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            //if (string.IsNullOrEmpty(token))
            //{
            //    return Unauthorized("Token bulunamadı.");
            //}

            var userName = await _tokenHelperService.GetUserNameFromToken(token);

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("Geçersiz token.");
            }
            var existingBook = await _borrowService.GetByNameAsync(userName);//?
            if (existingBook == null)
            {
                return NotFound();
            }

            await _borrowService.AddBorrowedBookAsync(userName, Id);

            return Ok("Kitap başarıyla ödünç alındı.");
        }


    }
}
