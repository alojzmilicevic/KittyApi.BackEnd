namespace KittyAPI.Hubs.Messages;

public class MessageTypes
{
    public const string IncomingCall = "incomingCall";
    public const string Offer = "offer";
    public const string IceCandidate = "iceCandidate";
    public const string Answer = "answer";
    public const string ViewerLeft = "hangup";
    public const string StreamsUpdated = "streamsUpdated";
}
