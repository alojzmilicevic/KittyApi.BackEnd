using KittyAPI.Services;
using Microsoft.AspNetCore.SignalR;

namespace KittyApi.Hubs;

public sealed class ClientType
{
    public static string Viewer = "viewer";
    public static string Streamer = "streamer";
}

public sealed class MessageType
{
    public static string ReceiveMessage = "ReceiveMessage";
}

public class ChatHub : Hub
{
    private readonly IStreamService _streamService;

    public ChatHub(IStreamService streamService) { 
        _streamService = streamService;


    }
    public override async Task OnConnectedAsync()
    {
        var userName = Context.User.Identity.Name;
        var clientType = Context?.GetHttpContext()?.Request?.Query["clientType"].ToString();

        if(clientType == ClientType.Viewer || clientType == ClientType.Streamer)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, clientType);
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userName = Context.User.Identity.Name;
        var clientType = Context?.GetHttpContext()?.Request?.Query["clientType"].ToString();

        if (clientType == ClientType.Viewer || clientType == ClientType.Streamer)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, clientType);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userName);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessageToStreamer(string user, Object message)
    {
        await Clients.Group(ClientType.Streamer).SendAsync(MessageType.ReceiveMessage, user, message);
    }

    public async Task SendMessageToAllViewers(string user, Object message)
    {
        await Clients.Group(ClientType.Viewer).SendAsync(MessageType.ReceiveMessage, user, message);
    }

    public async Task SendMessageToViewer(string from, string to, Object message)
    {
        await Clients.Client(to).SendAsync(MessageType.ReceiveMessage, from, message);
    }

    public async Task SendMessageToViewerBasedOnId(string from, string to, Object message)
    {
        await Clients.Group(to).SendAsync(MessageType.ReceiveMessage, from, message);
    }
}