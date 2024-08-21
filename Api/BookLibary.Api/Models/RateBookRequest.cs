using BookLibary.Api.Dtos.BookDto;

namespace BookLibary.Api.Models
{
    public class RateBookRequest 
    {
     //   public string UserName { get; set; }
        public string BookName { get; set; }
        public double Rating { get; set; }
    }

}
