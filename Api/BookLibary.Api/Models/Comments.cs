using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

public class Comments
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("UserName")]
    public required string  UserName { get; set; }

    [BsonElement("Comment")]
    public required string Comment { get; set; }

    [BsonElement("Status")]
    public required bool Status { get; set; }

    [BsonElement("BookName")]
    public required string BookName { get; set; }
}
