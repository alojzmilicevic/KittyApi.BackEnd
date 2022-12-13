namespace KittyAPI.Hubs.Messages;

public class Payload
{
    public string Type { get; set; } = string.Empty;

    public Payload(string type)
    {
        Type = type;
    }
}
