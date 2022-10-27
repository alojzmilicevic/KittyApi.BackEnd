using KittyAPI.Dto;
using KittyAPI.Dto.Stream;
using KittyAPI.Errors;
using KittyAPI.Hubs;
using KittyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Stream = KittyAPI.Models.Stream;

namespace KittyAPI.Services;

public interface IStreamService
{
    public StreamInfoDto? GetStreamInfo(string streamId);
    Task KickUserFromStream(UserDetailDto user, string streamId);
    Task<StreamInfoDto?> AddUserToStream(UserDetailDto user, string streamId);
    public Task StartStream(StartStreamDto body, UserDetailDto user);
    public Task EndStream(string streamId);

    public Task<List<StreamInfoDto>> GetAllStreams();

}
public class StreamService : IStreamService
{
    private readonly DataContext _dbContext;
    private readonly IHubService _hubService;
    private readonly IUserService _userService;
    public StreamService(DataContext dbContext, [FromServices] IUserService userService, [FromServices] IHubService hubService)
    {
        _hubService = hubService;
        _dbContext = dbContext;
        _userService = userService;
    }

    public async Task KickUserFromStream(UserDetailDto user, string streamId)
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

    public async Task StartStream(StartStreamDto body, UserDetailDto user)
    {
        var stream = await GetStreamFromDbContext(user.UserId);
        if (stream != null)
        {
            throw new StreamAlreadyLiveException();
        }

        stream = new Stream
        {
            StreamId = Guid.NewGuid().ToString(),
            IsActive = true,
            StreamerId = user.UserId,
            StreamTitle = body.StreamTitle,

        };

        _dbContext.Streams.Add(stream);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EndStream(string streamId)
    {
        var stream = await GetStreamFromDbContext(streamId);

        if (stream == null || !stream.IsActive)
        {
            throw new StreamNotLiveException();
        }

        _dbContext.Streams.Remove(stream);
        await _dbContext.SaveChangesAsync();
        //await _hubService.NotifyUsersInStream(body.StreamId);

    }

    private async Task<Stream> GetStreamFromDbContext(string streamId)
    {
        var stream = await _dbContext.Streams.Where(s => s.StreamId == streamId).FirstOrDefaultAsync();

        return stream;
    }

    public async Task<StreamInfoDto?> AddUserToStream(UserDetailDto user, string streamId)
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

    public StreamInfoDto? GetStreamInfo(string streamId)
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

    public async Task<List<StreamInfoDto>> GetAllStreams()
    {
        var streams = await _dbContext.Streams.Select(p => new StreamInfoDto()
        {
            StreamId = p.StreamId,
            StreamerId = p.StreamerId,
            isActive = p.IsActive,
            StreamTitle = p.StreamTitle,

        }).ToListAsync();

        return streams;
    }
}
