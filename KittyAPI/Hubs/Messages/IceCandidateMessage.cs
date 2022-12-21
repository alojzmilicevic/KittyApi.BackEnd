namespace KittyAPI.Hubs.Messages;

public class IceCandidateMessage : StreamHubMessage
{
    public IceCandidateMessage() : base(MessageTypes.IceCandidate) { }
    public string Candidate { get; set; } = string.Empty;
    public int SdpMLineIndex { get; set; } = 0;
    public string SdpMid { get; set; } = string.Empty;
}
