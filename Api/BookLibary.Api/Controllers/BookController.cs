using Microsoft.AspNetCore.Mvc;
using BookLibary.Api.Models;
using BookLibary.Api.Services.AuthServices.BookServices;

namespace BookLibary.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks(string name)
        {

             if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _bookService.GetAllBooksAsync(name);
             if (result== null)
            {
                return BadRequest(new { Message = "Kayıtlı kitap bulunamadı"});
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(string id)
        {
            var result = await _bookService.GetByIdAsync(id);
            if (result == null)
           {
                return BadRequest(new { Message = "Kayıtlı kitap bulunamadı"});
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            var result = await _bookService.CreateBookAsync(book);
            if (result== null)
            {
                return BadRequest(new { Message = "Kitap Eklenemedi "});
                
            }
            return Ok(CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book));
        }

        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateBook(string id, [FromBody] Book book)
        // {
        //     var existingBook = await _bookService.GetBookByIdAsync(id);
        //     if (existingBook.Entity == null)
        //     {
        //         return NotFound();
        //     }

        //     var result = await _bookService.UpdateBookAsync(id, book);
        //     if (result.Success)
        //     {
        //         return NoContent();
        //     }
        //     return BadRequest(result.Message);
        // }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(string id)
        {
            var result =  _bookService.DeleteBook(id);
            if (result.Success)
            {
                return NoContent();
            }
            return NotFound(new { Message = "Kitap Silinemedi"});
        }
    }
}
