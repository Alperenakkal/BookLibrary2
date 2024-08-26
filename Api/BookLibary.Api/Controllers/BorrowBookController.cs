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


        
         [HttpGet("GetBorrowBooks/{userName}")]
        public async Task<IActionResult> GetByNameAsync(string userName)
        {
            //var userId = await _tokenHelperService.GetIdFromToken();

            var borrowBooks = await _borrowService.GetBorrowBookAsync(userName);
            var result = new { BorrowBooks = borrowBooks };
            return Ok(result);
        }

        [HttpPost("AddBorrow/{userName}")]
        public async Task<IActionResult> AddBorrowedBook( BorrowBookByNameDto bookDto,string userName)
        {
            try
            {
                await _borrowService.AddBorrowedBookAsync(bookDto, userName);
                return Ok(new { message = "Kitap başarıyla ödünç alındı." });
            }
            catch (InvalidOperationException ex)
            {
                // Bu durumda kitap zaten okundu ama yine de ödünç alındı
                return Ok(new { message = ex.Message});
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu.", details = ex.Message });
            }
        }


        [HttpDelete("RemoveBorrowed/{userName}")]
        public async Task<IActionResult> RemoveBorrowedBookAsync(BorrowBookByNameDto bookDto, string userName)
        {
        

            await _borrowService.RemoveBookAsync(bookDto, userName);
            return Ok(new { message = "Kitap geri verildi" });
        }



        [HttpPut("UpdateBorrowedBook/{userName}")]
        public async Task<IActionResult> UpdateBook(BorrowBookByNameDto bookDto,string userName)
        {
            try
            {
                await _borrowService.AddtoReadoutBookAsync(bookDto, userName);
                return Ok(new { message = "Kitap Okunmuş Listenize Eklendi" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu.", details = ex.Message });
            }
        }


        [HttpGet("GetReadOutByName/{userName}")]
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
