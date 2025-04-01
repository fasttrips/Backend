using Trasgo.Shared.Models;

public class CreateOrderDto : BaseModel
{
    public PickupLocation? PickupLocation { get; set; }
    public DestinationLocation? DestinationLocation { get; set; }
    public string? IdDriver { get; set; }
    public string? IdMitra { get; set; }
    public string? IdUser { get; set; }
    public int? Status { get; set; }
    public string? Type { get; set; }///ini adalah type motor,mobil,atau mitra
    public string? Service { get; set; }///ini adalaha trasride,trasmove dll
    public bool? IsDeclinebyUser { get; set; }
    public string? NotesDecline { get; set; }
    public float? HargaLayanan { get; set; }
    public float? HargaPotonganDriver { get; set; }
    public float? HargaPotonganMitra { get; set; }
    public float? HargaKenaikan { get; set; }
    public float? Diskon { get; set; }
    public float? Jarak { get; set; }
    public string? Payment { get; set; }
    public List<Coordinate>? Coordinates { get; set; }

}

public class GetOrderDto
{
    public string? idOrder { get; set; }
}

public class CreateOrderWaDto
{
    public string? Phonenumber { get; set; }
}

public class PickupLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
}

public class Coordinate
{
    public double latitude { get; set; }
    public double longitude { get; set; }
}

public class Service
{
    public bool TrasRideCar { get; set; }
    public bool TrasRideCarXL { get; set; }
    public bool TrasRide { get; set; }
    public bool TrasRideXL { get; set; }
    public bool TrasRideTaxi { get; set; }
    public bool TrasMove { get; set; }
    public bool TrasFood { get; set; }

}

public class DestinationLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
}

public class ListDecline
{
    public string IdDriver { get; set; }
}