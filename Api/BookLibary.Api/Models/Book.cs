﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookLibary.Api.Models
 {
     public class Book:IBook
     {
         [BsonId]

         public ObjectId Id { get; set; }

         [BsonElement("BookName")]
         public  string BookName { get; set; } = string.Empty;
         [BsonElement("Publisher")]

         public string? Publisher {get; set;}
         [BsonElement("Author")]

         public string? Author { get; set; }
         [BsonElement("IsAvailable")]

         public int Stock { get; set; }


     }
 }