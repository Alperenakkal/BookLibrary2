using BookLibary.Api.Dtos.BookDto;

namespace BookLibary.Api.Models
{
    public class RateBookRequest 
    {
        public string BookId { get; set; }
        public double Rating { get; set; }
    }

}
