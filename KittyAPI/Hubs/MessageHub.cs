using KittyAPI.Errors;
using KittyAPI.Hubs.Messages;
using Microsoft.AspNetCore.SignalR;

namespace KittyApi.Hubs;

public sealed class ClientType
{
    public const string Viewer = "viewer";
    public const string Streamer = "streamer";
}

public sealed class MessageType
{
    public const string ReceiveMessage = "ReceiveMessage";
}

public interface IStreamHub
{
    Task ReceiveMessage(StreamHubMessage message);

    Task ReceiveMessage(StreamsUpdatedMessage message);
}

public class ChatHub : Hub<IStreamHub>
{
    public override async Task OnConnectedAsync()
    {
        var clientType = Context?.GetHttpContext()?.Request?.Query["clientType"].ToString();

        if (clientType == ClientType.Viewer || clientType == ClientType.Streamer)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, clientType);

        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var clientType = Context?.GetHttpContext()?.Request?.Query["clientType"].ToString();

        if (clientType == ClientType.Viewer || clientType == ClientType.Streamer)
        {
            if (Context != null && Context.ConnectionId != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, clientType);
            } else
            {
                throw new WebsocketError();
            }
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