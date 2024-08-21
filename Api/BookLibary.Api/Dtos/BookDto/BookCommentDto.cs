namespace BookLibary.Api.Dtos.BookDto
{
    public class BookCommentDto
    {

        public required string UserName { get; set; }
        public required string Comment { get; set; }
        public required bool Status { get; set; }
        public required string BookName { get; set; }
    }
}
