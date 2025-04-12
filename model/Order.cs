using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Trasgo.Shared.Models;

public class OrderModel : BaseModel
{
    [BsonId]
    // [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("PickupLocation")]
    public PickupLocation? PickupLocation { get; set; }

    [BsonElement("DestinationLocation")]
    public DestinationLocation? DestinationLocation { get; set; }

    [BsonElement("Makanan")]
    public List<OrderFood>? Makanan { get; set; }

    [BsonElement("IdDriver")]
    public string? IdDriver { get; set; }

    [BsonElement("IdMitra")]
    public string? IdMitra { get; set; }

    [BsonElement("IdUser")]
    public string? IdUser { get; set; }

    [BsonElement("Status")]
    public int? Status { get; set; }

    [BsonElement("Type")]
    public string? Type { get; set; }///ini adalah type motor,mobil,atau mitra

    [BsonElement("Service")]
    public string? Service { get; set; }///ini adalaha trasride,trasmove dll

    [BsonElement("IsDeclinebyUser")]
    public bool? IsDeclinebyUser { get; set; }

    [BsonElement("NotesDecline")]
    public string? NotesDecline { get; set; }

    [BsonElement("HargaLayanan")]
    public float? HargaLayanan { get; set; }

    [BsonElement("HargaPotonganDriver")]
    public float? HargaPotonganDriver { get; set; }

    [BsonElement("HargaPotonganMitra")]
    public float? HargaPotonganMitra { get; set; }

    [BsonElement("HargaKenaikan")]
    public float? HargaKenaikan { get; set; }

    [BsonElement("Diskon")]
    public float? Diskon { get; set; }

    [BsonElement("Jarak")]
    public float? Jarak { get; set; }

    [BsonElement("Payment")]
    public string? Payment { get; set; }

    [BsonElement("IdPendingPayment")]
    public string? IdPendingPayment { get; set; }

    [BsonElement("LastDriver")]
    public string? LastDriver { get; set; }

    [BsonElement("Coordinates")]
    public List<Coordinate>? Coordinates { get; set; }

}

public class PendingPaymentModel : BaseModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

   [BsonElement("IdUser")]
    public string IdUser { get; set; }

   [BsonElement("IdDriver")]
    public string IdDriver { get; set; }

   [BsonElement("IdMitra")]
    public string IdMitra { get; set; }

   [BsonElement("HargaLayanan")]
    public float? HargaLayanan { get; set; }

    [BsonElement("HargaPotonganDriver")]
    public float? HargaPotonganDriver { get; set; }

    [BsonElement("HargaPotonganMitra")]
    public float? HargaPotonganMitra { get; set; }

    [BsonElement("Diskon")]
    public float? Diskon { get; set; }
}
