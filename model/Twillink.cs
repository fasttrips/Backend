using MongoDB.Bson.Serialization.Attributes;
namespace Trasgo.Shared.Models
{
public class AddUrlTrasgo : BaseModel
    {
        [BsonId]
        public string? Title {get; set;}

        [BsonElement("UserId")]
        public string? UserId {get; set;}
    }
}