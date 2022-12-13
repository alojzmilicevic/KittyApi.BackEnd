namespace KittyAPI.Hubs.Messages;

public class StreamHubMessage
{
    public string Type { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public string Receiver { get; set; } = string.Empty;
    public StreamHubMessage(string type)
    {
        Type = type;
    }
}
