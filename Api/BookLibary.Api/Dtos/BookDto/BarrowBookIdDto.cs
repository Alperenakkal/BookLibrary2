using MongoDB.Bson;

namespace BookLibary.Api.Dtos.BookDto
{
    public class BarrowBookIdDto
    {
        public string Id { get; set; }

        // Optionally, if you need ObjectId in your code
        public DateTime BorrowedAt { get; set; }
      
    }
}
