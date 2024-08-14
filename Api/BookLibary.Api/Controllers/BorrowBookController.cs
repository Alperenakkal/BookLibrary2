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
        public async Task<IActionResult> GetByNameAsync(string id)
        {
            //var userId = await _tokenHelperService.GetIdFromToken();

            List<Book> book = await _borrowService.GetBorrowBookAsync(id);
            return Ok(book);
        }


        [HttpPost("AddBorrow")]
        public async Task<IActionResult> AddBorrowedBookAsync([FromBody] BarrowBookIdDto bookIdR, string id)
        {
         //  var userId= await _tokenHelperService.GetIdFromToken();
          
            var isAvailable = await _borrowService.IsBookAvailableAsync(bookIdR, id);
            
           
            await _borrowService.AddBorrowedBookAsync(bookIdR,id);
            return Ok(new { message = "Eklendi " });




        }
        [HttpDelete("RemoveBorrowed/{userId}")]
        public async Task<IActionResult> RemoveBorrowedBookAsync([FromBody] BarrowBookIdDto bookIdR, string userId)
        {
            // Gelen JSON verisini `BarrowBookIdDto` nesnesine çevirin
            if (bookIdR == null || string.IsNullOrEmpty(bookIdR.Id))
            {
                return BadRequest(new { message = "Book ID is required" });
            }

            await _borrowService.RemoveBookAsync(userId, bookIdR);
            return Ok(new { message = "Kitap geri verildi" });
        }




        [HttpPut("UpdateBorrowedBook")]
         public async Task<IActionResult> UpdateBook([FromBody] BarrowBookIdDto bookId, string id)
         {

            // var userId = await _tokenHelperService.GetIdFromToken();
             await _borrowService.AddtoReadoutBookAsync(bookId,id);




            
             return Ok(new { message = "Kitap okunmuş listesine eklendi" });
         }
         [HttpGet("GetReadOutByName")]
        public async Task<IActionResult> GetReadoutBookByNameAsync(string id)
        {
           // var userId = await _tokenHelperService.GetIdFromToken();

            var readOutBooks = await _borrowService.GetReadOutAsync(id);

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
