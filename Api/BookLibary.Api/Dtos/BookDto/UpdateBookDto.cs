using MongoDB.Bson;
 using MongoDB.Bson.Serialization.Attributes;

 namespace BookLibary.Api.Dtos.BookDto
 {
     public class UpdateBookDto
     {
         [BsonId]
         public ObjectId Id { get; set; }
         [BsonElement("BookName")]

         public string? BookName { get; set; }

         [BsonElement("Yazar")]

         public string? Yazar { get; set; }

         [BsonElement("Durum")]

         public bool Durum { get; set; }
     }
 }