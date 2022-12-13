namespace KittyAPI.Hubs.Messages;

public class AnswerMessage : StreamHubMessage
{
    public AnswerMessage() : base(MessageTypes.Answer) { }
    public string Sdp { get; set; } = string.Empty;
}
