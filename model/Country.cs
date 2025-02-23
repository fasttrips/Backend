using MongoDB.Bson.Serialization.Attributes;

namespace Trasgo.Shared.Models
{
    public class Country
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("name")]
        public string? name {get; set;}
        
        [BsonElement("flag")]
        public string? flag {get; set;}

        [BsonElement("code")]
        public string? code {get; set;}

        [BsonElement("dialCode")]
        public string? dialCode {get; set;}
        [BsonElement("createdAt")]
        public string? createdAt {get; set;}

        [BsonElement("updatedAt")]
        public string? updatedAt {get; set;}
    
    }
}