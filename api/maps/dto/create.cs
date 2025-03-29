public class CreateMapsDto
{
    public string? Name { get; set; }

}

public class CreateDirectionsDto
{
    public float? OriginLat { get; set; }
    public float? OriginLon { get; set; }
    public float? DestinationLat { get; set; }
    public float? DestinationLon { get; set; }


}