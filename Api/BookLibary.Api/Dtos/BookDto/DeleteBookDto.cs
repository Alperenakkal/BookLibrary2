using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookLibary.Api.Dtos.BookDto
 {
     public class DeleteBookDto
     {
         public ObjectId Id { get; set; }

         public string? Name { get; set; }

         [BsonElement("Yazar")]

         public string? Yazar { get; set; }

         [BsonElement("Durum")]

         public bool Durum { get; set; }
     }
 }