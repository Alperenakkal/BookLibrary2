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

        [BsonElement("Admin")]
        public bool IsAdmin { get; set; }

        [BsonElement("AvatarUrl")]
        public string avatarUrl { get; set; }

        [BsonElement("Gender")]
        public GenderType gender {  get; set; }


    }
}
