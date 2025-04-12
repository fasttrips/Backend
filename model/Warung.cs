using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trasgo.Shared.Models
{
    public class Warung : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("IdUser")]
        public string? IdUser { get; set; }

        [BsonElement("FullName")]
        public string? FullName { get; set; }

        [BsonElement("ImageCover")]
        public string? ImageCover { get; set; }

        [BsonElement("Address")]
        public string? Address { get; set; }

        [BsonElement("Latitude")]
        public double? Latitude { get; set; }

        [BsonElement("Longitude")]
        public double? Longitude { get; set; }

        [BsonElement("IsUMKM")]
        public bool? IsUMKM { get; set; }

        [BsonElement("Category")]
        public List<string>? Category { get; set; }

        [BsonElement("JamBuka")]
        public DateTime? JamBuka { get; set; }

        [BsonElement("JamTutup")]
        public DateTime? JamTutup { get; set; }
    }

    public class Food : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }
        
        [BsonElement("ImageCover")]
        public string? ImageCover { get; set; }

        [BsonElement("IdUser")]
        public string? IdUser { get; set; }

        [BsonElement("FullName")]
        public string? FullName { get; set; }

        [BsonElement("Description")]
        public string? Description { get; set; }

        [BsonElement("Price")]
        public int? Price { get; set; }

        [BsonElement("Diskon")]
        public int? Diskon { get; set; }

        [BsonElement("IsAvailable")]
        public bool? IsAvailable { get; set; }
    }

    public class OrderFood
    {
        [BsonId]
        public string? Id { get; set; }
        
        [BsonElement("ImageCover")]
        public string? ImageCover { get; set; }

        [BsonElement("FullName")]
        public string? FullName { get; set; }

        [BsonElement("Price")]
        public int? Price { get; set; }
        
        [BsonElement("Diskon")]
        public int? Diskon { get; set; }

        [BsonElement("Jumlah")]
        public int? Jumlah { get; set; }

        [BsonElement("Notes")]
        public string? Notes { get; set; }
    }
}