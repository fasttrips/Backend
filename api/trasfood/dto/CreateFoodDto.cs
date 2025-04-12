public class CreateWarungDto
{
    public string? IdUser { get; set; }
    public string? FullName { get; set; }
    public string? ImageCover { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool? IsUMKM { get; set; }
    public List<string>? Category { get; set; }

    public DateTime? JamBuka { get; set; }
    public DateTime? JamTutup { get; set; }
}

public class CreateFoodDto
{
    public string? IdUser { get; set; }
    public string? FullName { get; set; }
    public string? ImageCover { get; set; }
    public string? Description { get; set; }
    public int? Price { get; set; }
    public int? Diskon { get; set; }
    public bool? IsAvailable { get; set; }


}