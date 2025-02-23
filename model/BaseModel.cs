using MongoDB.Bson.Serialization.Attributes;

namespace Trasgo.Shared.Models
{
    public class BaseModel
    {
        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }
        [BsonElement("IsActive")]
        public bool? IsActive { get; set; }
        [BsonElement("IsVerification")]
        public bool? IsVerification {get; set;}

    }
}