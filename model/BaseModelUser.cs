using MongoDB.Bson.Serialization.Attributes;

namespace Trasgo.Shared.Models
{
    public class BaseModelUser
    {
        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }
        [BsonElement("UserId")]
        public string? UserId {get; set;}
        [BsonElement("sequence")]
        public long? sequence {get; set;}
        [BsonElement("typeWidget")]
        public string? typeWidget {get; set;}
        [BsonElement("width")]
        public string? width {get; set;}
    }
}