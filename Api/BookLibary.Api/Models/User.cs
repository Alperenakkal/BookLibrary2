using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookLibary.Api.Models
{
    public class User:IUser
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("UserName")]
        public string UserName { get; set; }
        [BsonElement("FullName")]
        public string FullName { get; set; }
        [BsonElement("Password")]
        public string Password { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }


        [BsonElement("BorrowBooks")]
        public List<ObjectId> BorrowBooks { get; set; } = new List<ObjectId>();
        [BsonElement("ReadOutBooks")]
        public List<ObjectId> ReadOutBooks {get; set;} = new List<ObjectId>();

//        [BsonElement("ReadingBooks")]
//        public List<string> ReadingBooks { get; set; } = new List<string>();


        [BsonElement("Admin")]
        public bool IsAdmin { get; set; }


    }
}
