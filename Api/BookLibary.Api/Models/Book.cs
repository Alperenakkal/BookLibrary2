using BookLibary.Api.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BookLibary.Api.Models
{
    public class UserRating
    {
        public string UserName { get; set; }
        public double Rating { get; set; }
    }
    public class Book : IBook
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("BookName")]
        public string BookName { get; set; } 

        [BsonElement("Publisher")]
        public string? Publisher { get; set; }

        [BsonElement("Author")]
        public string? Author { get; set; }

        [BsonElement("CoverImageUrl")]
        public string CoverImageUrl { get; set; }

        [BsonElement("Category")]
        public string Category { get; set; }

        [BsonElement("Stock")]
        public int Stock { get; set; }

        [BsonElement("IsAvailable")]
        public bool IsAvailable { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; }

        [BsonElement("AverageRating")]
        public double AverageRating { get; set; }

        [BsonElement("RatingCount")]
        public int RatingCount { get; set; }
        public List<Ratings> Ratings { get; set; }


    }
}
