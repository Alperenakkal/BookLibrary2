using MongoDB.Bson.Serialization.Attributes;

namespace BookLibary.Api.Models
{
public interface IBook
    {
        [BsonElement("BookName")]
        public string BookName { get; set; }

        [BsonElement("Ratings")]
        public List<Ratings> Ratings { get; set; } 
    }
}