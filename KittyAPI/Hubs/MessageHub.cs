using KittyAPI.Hubs;
using KittyAPI.Hubs.Messages;
using KittyAPI.Services;
using Microsoft.AspNetCore.Mvc;
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

public interface IStreamHub
{
    Task ReceiveMessage(StreamHubMessage message);
}

public class ChatHub : Hub<IStreamHub>
{
    private readonly IStreamService _streamService;
    private readonly IUserService _userService;

    public ChatHub(IStreamService streamService, [FromServices] IUserService userService)
    {
        _streamService = streamService;
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        var clientType = Context?.GetHttpContext()?.Request?.Query["clientType"].ToString();

        if (clientType == ClientType.Viewer || clientType == ClientType.Streamer)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, clientType);

        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var clientType = Context?.GetHttpContext()?.Request?.Query["clientType"].ToString();

        if (clientType == ClientType.Viewer || clientType == ClientType.Streamer)
        {
            if (clientType == ClientType.Viewer)
            {
                var user = _userService.GetUserFromContext(Context.GetHttpContext());
                //await _streamService.KickUserFromStream(user, 1);
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, clientType);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendOfferMessage(OfferMessage message)
    {
        await Clients.User(message.Receiver).ReceiveMessage(message);
    }

    public async Task SendAnswerMessage(AnswerMessage message)
    {
        await Clients.Group(ClientType.Streamer).ReceiveMessage(message);
    }

    public async Task SendIceCandidateMessage(IceCandidateMessage message)
    {
        await Clients.User(message.Receiver).ReceiveMessage(message);
    }
}