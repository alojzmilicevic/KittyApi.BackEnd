using KittyAPI.Dto;
using KittyAPI.Errors;
using KittyAPI.Hubs;
using KittyAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace KittyAPI.Services;

public interface IStreamService
{
    public StreamInfoDto? GetStreamInfo(int streamId);
    Task KickUserFromStream(UserDetailDto user, int streamId);
    Task<StreamInfoDto?> AddUserToStream(UserDetailDto user, int streamId);
    public Task StartStream(UserJoinStreamDto body);
    public Task EndStream(UserJoinStreamDto body);

}
public class StreamService : IStreamService
{
    private readonly DataContext _dbContext;
    private readonly IHubService _hubService;

    public StreamService(DataContext dbContext, [FromServices] IUserService userService, [FromServices] IHubService hubService)
    {
        _hubService = hubService;
        _dbContext = dbContext;
    }

    public async Task KickUserFromStream(UserDetailDto user, int streamId)
    {
        var tmpUser = _dbContext.StreamUsers.Where(x => x.UserId == user.UserId);
        if (!tmpUser.Any()) return;

        _dbContext.StreamUsers.Remove(new StreamUser()
        {
            StreamId = streamId,
            UserId = user.UserId,
        });

        _dbContext.SaveChanges();
        var message = new SignalRMessageDto("hangup");
        await _hubService.SendMessageToStreamer(user.UserId, message);

    }

    public async Task StartStream(UserJoinStreamDto body)
    {
        var stream = await GetStreamFromDbContext(body.StreamId);
        if (stream.IsActive)
        {
            throw new StreamAlreadyLiveException();
        }

        stream.IsActive = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task EndStream(UserJoinStreamDto body)
    {
        var stream = await GetStreamFromDbContext(body.StreamId);

        if(!stream.IsActive)
        {
            throw new StreamNotLiveException();
        }

        stream.IsActive = false;
        await _dbContext.SaveChangesAsync();
        //await _hubService.NotifyUsersInStream(body.StreamId);

    }

    private async Task<Models.Stream> GetStreamFromDbContext(int streamId)
    {
        var stream = await _dbContext.Streams.Where(s => s.StreamId == streamId).FirstOrDefaultAsync();

        if (stream == null)
        {
            throw new StreamNotFoundException();
        }

        return stream;
    }

    public StreamInfoDto? GetStreamInfo(int streamId)
    {
        var streamInfo = _dbContext.Streams.Where(s => s.StreamId == streamId).Select(p => new StreamInfoDto()
        {
            StreamId = p.StreamId,
            isActive = p.IsActive,
            StreamTitle = p.StreamTitle,
            Users = p.Participants.Select(n => n.User).Select(n => new UserInfoSharedDto()
            {
                FirstName = n.FirstName,
                LastName = n.LastName,
                Username = n.Username,
            }).ToList()
        }).FirstOrDefault();

        return streamInfo;
    }

    public async Task<StreamInfoDto?> AddUserToStream(UserDetailDto user, int streamId)
    {
        try
        {
            await _dbContext.StreamUsers.AddAsync(new StreamUser()
            {
                StreamId = streamId,
                UserId = user.UserId,
            });

            await _dbContext.SaveChangesAsync();

            var message = new SignalRMessageDto("incomingCall");
            await _hubService.SendMessageToStreamer(user.UserId, message);

            return GetStreamInfo(streamId);

        }
        catch (DbUpdateException)
        {
            throw new UserAlreadyInStreamException();
        }
    }
}
