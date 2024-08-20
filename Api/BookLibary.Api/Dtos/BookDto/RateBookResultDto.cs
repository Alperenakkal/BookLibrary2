namespace BookLibary.Api.Dtos.BookDto
{
    public class RateBookResultDto
    {
        public bool Success { get; set; }
        public double AverageRating { get; set; }
        public string Message { get; set; }
    }
}
