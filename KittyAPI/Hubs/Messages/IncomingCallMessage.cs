namespace KittyAPI.Hubs.Messages;

public class IncomingCallMessage : StreamHubMessage
{
    public IncomingCallMessage(string sender) : base(MessageTypes.IncomingCall) {
        Sender = sender;
        Receiver = "streamer";
    }
}
