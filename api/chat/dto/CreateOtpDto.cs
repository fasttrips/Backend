public class CreateChatDto
{
    public string? IdOrder { get; set; }
    public string? IdUser { get; set; }
    public string? IdDriver { get; set; }
    public string? Message { get; set; }

}

public class GetChatDto
{
    public string? IdOrder { get; set; }
    public string? IdUser { get; set; }
    public string? IdDriver { get; set; }

}