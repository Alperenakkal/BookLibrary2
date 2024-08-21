using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Middleware;
using BookLibary.Api.Models;
using BookLibary.Api.Models.Request.CommentRequest;
using BookLibary.Api.Models.Response.CommentResponse;
using BookLibary.Api.Repositories;
using System.Xml.Linq;

namespace BookLibary.Api.Services.AuthServices.CommentService
{
    public class CommentService : ICommentService
    {

        private readonly ICommentRepository<Comments> _commentRepository;

        public CommentService(ICommentRepository<Comments> commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<AddCommentResponse> CreateCommentAsync(string bookName, AddCommentRequest comment)
        {
            try
            {
                if (bookName == null)
                {
                    throw new BadRequestException($"{bookName} is not empty ");
                }
                if (comment == null)
                {
                    throw new BadRequestException($"{comment} bad request");
                }
                Comments newComments = new Comments
                {
                    BookName = bookName,
                    Comment = comment.Comment,
                    Status = comment.Status,
                    UserName = comment.UserName,

                };

                Comments ınsertComment = await _commentRepository.InsertOneAsync(newComments);
                AddCommentResponse commentResponse = new AddCommentResponse
                {
                    BookName = bookName,
                    Comment = ınsertComment.Comment,
                    Status = ınsertComment.Status,
                    UserName = ınsertComment.UserName,
                };
                return commentResponse;
            }
            catch (Exception)
            {
                throw new NotFoundException($"Not Found CreatCommentService");

            }

        }
        public async Task<List<BookCommentDto>> GetCommentByBookName(string bookName)
        {
            try
            {
                if (string.IsNullOrEmpty(bookName))
                {
                    throw new BadRequestException($"{bookName} is not empty");
                }

                // Yorumlar listesini alıyoruz
                List<Comments> commentList = await _commentRepository.GetCommentByBookName(bookName);

                if (commentList == null || !commentList.Any())
                {
                    throw new NotFoundException($"CommentsRepositoryden beklenen değer gelmedi");
                }

                // Dönüş olarak kullanılacak BookCommentDto türünde bir liste oluşturuyoruz
                List<BookCommentDto> bookCommentDtoList = new List<BookCommentDto>();

                // Yorum listesindeki her bir yorumu dönüştürüp, listeye ekliyoruz
                foreach (var comments in commentList)
                {
                    BookCommentDto bookCommentDto = new BookCommentDto
                    {
                        BookName = comments.BookName,
                        Comment = comments.Comment,
                        Status = comments.Status,
                        UserName = comments.UserName,
                    };

                    bookCommentDtoList.Add(bookCommentDto);
                }

                return bookCommentDtoList;
            }
            catch (Exception)
            {
                throw new NotFoundException($"GetCommentByBookNameService[CommentService 57] not found");
            }
        }
    }
    }
