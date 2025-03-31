using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class OrderModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("Email")]
    public string Email { get; set; }
    [BsonElement("Phone")]
    public string Phone { get; set; }
    [BsonElement("CodeOrder")]
    public string CodeOrder { get; set; }
    [BsonElement("TypeOrder")]
    public string TypeOrder { get; set; }
    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; }
}
