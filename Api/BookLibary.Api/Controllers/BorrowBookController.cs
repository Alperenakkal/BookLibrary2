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
        public async Task<IActionResult> GetByNameAsync()
        {
            var userId = await _tokenHelperService.GetIdFromToken();

            List<Book> book = await _borrowService.GetBorrowBookAsync(userId);
            return Ok(book);
        }


        [HttpPost("AddBorrow")]
        public async Task<IActionResult> AddBorrowedBookAsync([FromBody] BarrowBookIdDto bookIdR)
        {
           var userId= await _tokenHelperService.GetIdFromToken();
          
            var isAvailable = await _borrowService.IsBookAvailableAsync(bookIdR);
            
           
            await _borrowService.AddBorrowedBookAsync(bookIdR,userId);
            return Ok("Kitap başarıyla ödünç alındı.");
            
            
            
            
        }
        [HttpDelete("RemoveBorrowed")]
        public async Task<IActionResult> RemoveBorrowedBookAsync([FromBody] BarrowBookIdDto bookIdR)
        {
            var userId = await _tokenHelperService.GetIdFromToken();
            await _borrowService.RemoveBookAsync(userId, bookIdR);

            return Ok("Kitap başarıyla geri verildi.");

        }

         [HttpPut("UpdateBorrowedBook")]
         public async Task<IActionResult> UpdateBook([FromBody] BarrowBookIdDto bookId)
         {

             var userId = await _tokenHelperService.GetIdFromToken();
             await _borrowService.AddtoReadoutBookAsync(bookId,userId);




            
             return Ok("Kitap Okunmuş Listenize Eklendi");
         }
         [HttpGet("GetReadOutByName")]
        public async Task<IActionResult> GetReadoutBookByNameAsync()
        {
            var userId = await _tokenHelperService.GetIdFromToken();

            var readOutBooks = await _borrowService.GetReadOutAsync(userId);

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
