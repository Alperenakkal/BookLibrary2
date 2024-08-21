using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Models.Request.CommentRequest;
using BookLibary.Api.Models.Response.CommentResponse;

namespace BookLibary.Api.Services.AuthServices.CommentService
{
    public interface ICommentService
    {
        Task<AddCommentResponse> CreateCommentAsync(string bookName, AddCommentRequest comment);
       Task<List<BookCommentDto>> GetCommentByBookName(string bookName);
    }
}
