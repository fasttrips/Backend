using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trasgo.Shared.Models
{
    public class Role : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; }
    }
}