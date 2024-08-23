namespace BookLibary.Api.Dtos.BookDto
{
    public class UserBookRatingDto
    {
        public bool Success { get; set; }
        public double? UserRating { get; set; }
        public string Message { get; set; }
    }

}
