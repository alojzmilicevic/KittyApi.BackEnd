namespace KittyAPI.Hubs.Messages;

public class OfferMessage : StreamHubMessage
{
    public OfferMessage() : base(MessageTypes.Offer) { }
    public string Sdp { get; set; } = string.Empty;
}
