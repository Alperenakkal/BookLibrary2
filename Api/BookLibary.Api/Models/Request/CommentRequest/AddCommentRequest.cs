using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BookLibary.Api.Models.Request.CommentRequest
{
    public class AddCommentRequest
    {
       public required string UserName { get; set; }

   
        public required string Comment { get; set; }

        public required bool Status { get; set; }

    }
}
