using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Trasgo.Shared.Models;

public class ChatModel : BaseModel
{
    [BsonId]
    public string Id { get; set; }

    [BsonElement("IdOrder")]
    public string? IdOrder { get; set; }

    [BsonElement("IdUser")]
    public string? IdUser { get; set; }

    [BsonElement("IdDriver")]
    public string? IdDriver { get; set; }
    [BsonElement("Sender")]
    public string? Sender { get; set; }

    [BsonElement("Message")]
    public string? Message { get; set; }
}
