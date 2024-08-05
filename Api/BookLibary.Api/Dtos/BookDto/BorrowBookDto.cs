using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookLibary.Api.Dtos.BookDto
 {
     public class BorrowBookDto
     {
         public ObjectId Id { get; set; }


         public string? Name { get; set; }
         [BsonElement("Yazar")]

         public string? Yazar { get; set; }

         [DataMember]
         [BsonElement("IsAvailable")]

         public bool IsAvailable { get; set; } 

         public DateTime BorrowDate {get; set;}

         public DateTime DueDate{get; set;}
     }
 }