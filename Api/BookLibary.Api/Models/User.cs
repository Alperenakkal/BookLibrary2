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
        public bool IsAvailable { get; set; } = true; // Kullanıcı için varsayılan olarak true
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
        public List<string> ReadOutBooks { get; set; } = new List<string>();

        [BsonElement("Admin")]
        public bool IsAdmin { get; set; }

        [BsonElement("AvatarUrl")]
        public string avatarUrl { get; set; }

        [BsonElement("Gender")]
        public GenderType gender {  get; set; }
        


    }
}
