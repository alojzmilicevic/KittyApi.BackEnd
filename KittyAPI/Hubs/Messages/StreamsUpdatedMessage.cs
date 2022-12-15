using KittyAPI.Dto.Stream;

namespace KittyAPI.Hubs.Messages;

public class StreamsUpdatedMessage : StreamHubMessage
{
    public List<StreamInfoDto> Streams { get; set; }
    public StreamsUpdatedMessage(List<StreamInfoDto> streams) : base(MessageTypes.StreamsUpdated)
    {
        Streams = streams;
    }
}
