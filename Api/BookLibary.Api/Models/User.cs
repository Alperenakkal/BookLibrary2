using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookLibary.Api.Models
{

    public enum GenderType
    {   
        other,
        Male,
        Female
    }

    public class BorrowedBook
    {
        public string BookName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true; 
    }
    public class ReadoutBook
    {
        public string BookName { get; set; }
        public int BorrowCount { get; set; }
    }

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
        public List<BorrowedBook> BorrowBooks { get; set; } = new List<BorrowedBook>();

        [BsonElement("ReadOutBooks")]
        public List<ReadoutBook> ReadOutBooks { get; set; } = new List<ReadoutBook>();

        [BsonElement("Admin")]
        public bool IsAdmin { get; set; }

        [BsonElement("AvatarUrl")]
        public string avatarUrl { get; set; }

        [BsonElement("Gender")]
        public GenderType gender {  get; set; }
        


    }
}
