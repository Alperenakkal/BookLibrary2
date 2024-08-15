using Azure.Core;
using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Services.AuthServices;
using BookLibary.Api.Services.AuthServices.BookServices;
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

        private readonly IBookService _bookService;



        public BorrowBookController(IBorrowService borrowService, ITokenHelperService tokenHelperService, IUpdateService updateService, IBookService bookService)
        {
            _borrowService = borrowService;
            _tokenHelperService = tokenHelperService;
            _updateService = updateService;
            _bookService = bookService;

        }


        
         [HttpGet("GetBorrowBooks")]
        public async Task<IActionResult> GetByNameAsync(string userName)
        {
            //var userId = await _tokenHelperService.GetIdFromToken();

            var borrowBooks = await _borrowService.GetBorrowBookAsync(userName);
            var result = new { BorrowBooks = borrowBooks };
            return Ok(result);
        }

        [HttpPost("AddBorrow")]
        public async Task<IActionResult> AddBorrowedBookAsync([FromBody] BorrowBookByNameDto bookDto, [FromQuery] string userName)
        {
         //  var userId= await _tokenHelperService.GetIdFromToken();
          
            var isAvailable = await _borrowService.IsBookAvailableAsync(bookDto, userName);
            
           
            await _borrowService.AddBorrowedBookAsync(bookDto, userName);
            return Ok(new { message = "Eklendi " });




        }
        [HttpDelete("RemoveBorrowed")]
        public async Task<IActionResult> RemoveBorrowedBookAsync([FromBody] BorrowBookByNameDto bookDto, [FromQuery] string userName)
        {
        

            await _borrowService.RemoveBookAsync(bookDto, userName);
            return Ok(new { message = "Kitap geri verildi" });
        }




        [HttpPut("UpdateBorrowedBook")]
        public async Task<IActionResult> UpdateBook([FromBody] BorrowBookByNameDto bookDto, [FromQuery] string userName)
        {
            await _borrowService.AddtoReadoutBookAsync(bookDto, userName);
            return Ok(new { message = "Kitap Okunmuş Listenize Eklendi" });
        }

        [HttpGet("GetReadOutByName")]
        public async Task<IActionResult> GetReadoutBookByNameAsync(string userName)
        {
           // var userId = await _tokenHelperService.GetIdFromToken();

            var readOutBooks = await _borrowService.GetReadOutAsync(userName);

            var result = new
            {
             ReadOutBooks = readOutBooks
            };

            return Ok(result);
        }
        [HttpGet("user/{id}")]
        public async Task<IActionResult> IdGetUser(string id)
        {

            var user = await _borrowService.GetByIdAsync(id);



            return Ok(user);
        }


    }
}
