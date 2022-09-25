using KittyApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace KittyAPI.Hubs;

public interface IHubService {
    Task SendMessageToStreamer(string user, Object message);
    Task SendMessageToViewerBasedOnUserName(string user, string to, Object message);
    Task SendMessageToViewersInStream(int streamId);
}

public class HubService : IHubService {
    private readonly IHubContext<ChatHub, IStreamHub> _hubContext;

    public HubService(IHubContext<ChatHub, IStreamHub> hubContext) {
        _hubContext = hubContext;
    }

    public async Task SendMessageToStreamer(string user, Object message)
    {
        await _hubContext.Clients.Group(ClientType.Streamer).ReceiveMessage(user, message);
    }

    public async Task SendMessageToViewerBasedOnUserName(string from, string to, Object message)
    {
        await _hubContext.Clients.User(to).ReceiveMessage(from, message);
    }

    public async Task SendMessageToViewersInStream(int streamId)
    {
        await _hubContext.Clients.Group(streamId.ToString()).ReceiveMessage(ClientType.Streamer, "Test.StreamEnded");
    }
}
