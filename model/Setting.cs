using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trasgo.Shared.Models
{
    public class Setting : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("Key")]
        public string? Key { get; set; }
        [BsonElement("Value")]
        public string? Value { get; set; }
    }
}