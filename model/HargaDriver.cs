using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trasgo.Shared.Models
{
    public class HargaDriver : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("Harga")]
        public int? Harga { get; set; }

        [BsonElement("KenaikanHarga")]
        public int? KenaikanHarga { get; set; }

        [BsonElement("Icon")]
        public string? Icon { get; set; }

        [BsonElement("Potongan1")]
        public int? Potongan1 { get; set; }

        [BsonElement("Potongan2")]
        public int? Potongan2 { get; set; }

        [BsonElement("Potongan3")]
        public int? Potongan3 { get; set; }
        [BsonElement("Type")]
        public string? Type { get; set; }
        [BsonElement("Desc")]
        public string? Desc { get; set; }
        [BsonElement("Penumpang")]
        public string? Penumpang { get; set; }
        [BsonElement("Durasi")]
        public string? Durasi { get; set; }
        [BsonElement("IsHemat")]
        public bool? IsHemat { get; set; }
        [BsonElement("IsPremium")]
        public bool? IsPremium { get; set; }
    }
}