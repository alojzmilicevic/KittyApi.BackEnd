namespace KittyAPI.Hubs.Messages;

public class LeaveStreamMessage : StreamHubMessage
{
    public LeaveStreamMessage(string sender) : base(MessageTypes.ViewerLeft)
    {
        Sender = sender;
        Receiver = "streamer";
    }
}
