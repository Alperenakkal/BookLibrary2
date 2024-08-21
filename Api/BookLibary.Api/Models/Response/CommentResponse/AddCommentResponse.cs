using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BookLibary.Api.Models.Response.CommentResponse
{
    public class AddCommentResponse
    {
      
        public required string UserName { get; set; }
        public required string Comment { get; set; }
        public required bool Status { get; set; }
        public required string BookName { get; set; }
    }
}
