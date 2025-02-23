using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trasgo.Shared.Models
{
    public class Payment : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("TypePayment")]
        public string? TypePayment { get; set; }

        [BsonElement("IdUser")]
        public string? IdUser { get; set; }
        [BsonElement("Photo")]
        public string? Photo { get; set; }
        [BsonElement("NameUser")]
        public string? NameUser { get; set; }
        [BsonElement("LastUser")]
        public string? LastUser { get; set; }
         [BsonElement("Email")]
        public string? Email { get; set; }
         [BsonElement("Phone")]
        public string? Phone { get; set; }
        [BsonElement("PasswordRoom")]
        public string? PasswordRoom { get; set; }

        [BsonElement("IdItem")]
        public string? IdItem { get; set; }

        [BsonElement("Price")]
        public float? Price { get; set; }
    }
}