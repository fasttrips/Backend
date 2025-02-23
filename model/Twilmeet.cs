using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trasgo.Shared.Models
{
    public class Twilmeet : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("Thumbnail")]
        public string? Thumbnail { get; set; }

        [BsonElement("Type")]
        public string? Type { get; set; }

        [BsonElement("IdUser")]
        public string? IdUser { get; set; }

        [BsonElement("Title")]
        public string? Title { get; set; }

        [BsonElement("Desc")]

        public string? Desc { get; set; }
        [BsonElement("Date")]

        public string? Date { get; set; }
        [BsonElement("Time")]

        public string? Time { get; set; }
        [BsonElement("Category")]

        public string? Category { get; set; }
        [BsonElement("Languange")]

        public string? Languange { get; set; }
        [BsonElement("Tags")]

        public string? Tags { get; set; }
        [BsonElement("IsPaid")]

        public bool? IsPaid { get; set; }
        [BsonElement("IsCertificate")]

        public bool? IsCertificate { get; set; }
        [BsonElement("IsClass")]

        public bool? IsClass { get; set; }
        [BsonElement("Class")]

        public string[]? Classes { get; set; }
        [BsonElement("Price")]

        public float? Price { get; set; }
    }
}