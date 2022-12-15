using KittyApi.Hubs;
using KittyAPI.Dto.Stream;
using KittyAPI.Hubs.Messages;
using Microsoft.AspNetCore.SignalR;

namespace KittyAPI.Hubs;

public interface IHubService
{
    Task SendIncomingCallMessage(string userId);
    Task SendHangupMessage(string userId);
    Task SendStreamsUpdatedMessage(List<StreamInfoDto> streams);
}

public class HubService : IHubService
{
    private readonly IHubContext<ChatHub, IStreamHub> _hubContext;

    public HubService(IHubContext<ChatHub, IStreamHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendIncomingCallMessage(string userId)
    {
        var message = new IncomingCallMessage(userId);

        await _hubContext.Clients.Group(ClientType.Streamer).ReceiveMessage(message);
    }

    public async Task SendHangupMessage(string userId)
    {
        var message = new LeaveStreamMessage(userId);

        await _hubContext.Clients.Group(ClientType.Streamer).ReceiveMessage(message);
    }

    public async Task SendStreamsUpdatedMessage(List<StreamInfoDto> streams)
    {
        var message = new StreamsUpdatedMessage(streams);

        await _hubContext.Clients.Group(ClientType.Viewer).ReceiveMessage(message);
    }
}
