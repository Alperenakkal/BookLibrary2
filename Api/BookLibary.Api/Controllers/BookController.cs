using Microsoft.AspNetCore.Mvc;
using BookLibary.Api.Models;
using BookLibary.Api.Services.AuthServices.BookServices;
using BookLibary.Api.Dtos.BookDto;
using System.Threading.Tasks;
using BookLibary.Api.Services.AuthServices.CommentService;
using BookLibary.Api.Models.Request.CommentRequest;

namespace BookLibary.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ICommentService _commentService;
        public BookController(IBookService bookService,ICommentService commentService)
        {
            _bookService = bookService;
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _bookService.GetAllBooksAsync();
            if (result == null)
            {
                return BadRequest(new { Message = "Kayıtlı kitap bulunamadı" });
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(string id)
        {
            var result = await _bookService.GetByIdAsync(id);
            if (result == null)
            {
                return BadRequest(new { Message = "Kayıtlı kitap bulunamadı" });
            }
            return Ok(result);
        }

        [HttpGet("Name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _bookService.GetByNameAsync(name);
            return Ok(result);
        }
        [HttpPost("addComment/{bookName}")]
        public async Task<IActionResult> CreateComment(string bookName,[FromBody] AddCommentRequest comments)
        {
            var result = await _commentService.CreateCommentAsync(bookName,comments);
            return Ok(result);

        }
        [HttpGet("getComment/{bookName}")]
        public async Task<IActionResult> GetCommentByBookName(string bookName)
        {
            var result = await _commentService.GetCommentByBookName(bookName);
          
            return Ok(result);

        }
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            var result = await _bookService.CreateBookAsync(book);
            if (result == null)
            {
                return BadRequest(new { Message = "Kitap Eklenemedi " });
            }
            return Ok(CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book));
        }

        [HttpDelete("{bookName}")]
        public async Task<IActionResult> DeleteBook(string bookName)
        {
            var result = await _bookService.DeleteBook(bookName);
            if (result.Success)
            {
                return NoContent();
            }
            return NotFound(new { Message = "Kitap Silinemedi" });
        }

        //[HttpPost("rate-book")]
        //public async Task<IActionResult> RateBook([FromBody] RateBookRequest request)
        //{
        //    var result = await _bookService.RateBookAsync(request);
        //    if (!result.Success)
        //    {
        //        return NotFound(new { Message = result.Message });
        //    }
        //    return Ok(new { message = "Puan kaydedildi.", averageRating = result.AverageRating });
        //}


    }
}
