using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Trasgo.Shared.Models;

public class DriverAvalibleModel
{
    [BsonId]
    // [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("Service")]
    public Service? Service { get; set; }

    [BsonElement("Latitude")]
    public double? Latitude { get; set; }

    [BsonElement("Longitude")]
    public double? Longitude { get; set; }

    [BsonElement("IsActive")]
    public bool? IsActive { get; set; }
    [BsonElement("IsStandby")]
    public bool? IsStandby { get; set; }
    [BsonElement("OnCall")]
    public bool? OnCall { get; set; }

    [BsonElement("FCM")]
    public string FCM { get; set; } 

    [BsonElement("LastActive")]
    public DateTime LastActive { get; set; }

    [BsonElement("Plat")]
    public string Plat { get; set; } 

    [BsonElement("Rating")]
    public string Rating { get; set; }

    [BsonElement("NamaKendaraan")]
    public string NamaKendaraan { get; set; }
}

public class DriverListCancelModel
{
    [BsonId]
    // [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("IdDriver")]
    public string? IdDriver { get; set; }

    [BsonElement("IdOrder")]
    public string? IdOrder { get; set; }

    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; }
}