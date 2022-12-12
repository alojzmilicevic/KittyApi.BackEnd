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
    public StreamInfoDto? GetStreamInfoBasedOnStreamer(string streamerId);
    Task KickUserFromStream(UserDetailDto user, string streamId);
    Task<StreamInfoDto?> AddUserToStream(UserDetailDto user, string streamId);
    public Task<string> StartStream(StartStreamDto body, UserDetailDto user);
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

    public async Task<string> StartStream(StartStreamDto body, UserDetailDto user)
    {
        var stream = _dbContext.Streams.Where(s => s.Streamer.UserId == user.UserId).SingleOrDefault();
        if (stream != null)
        {
            await EndStream(stream.StreamId);
        }


        //TODO add error handling
        var actualUser = await _dbContext.Users.FindAsync(user.UserId);
        var thumbnail = await _dbContext.Thumbnails.FindAsync(body.ThumbnailId);
        string streamId = Guid.NewGuid().ToString();
        stream = new Stream
        {
            StreamId = streamId,
            IsActive = true,
            Streamer = actualUser,
            StreamTitle = body.StreamTitle,
            Thumbnail = thumbnail,
        };

        _dbContext.Streams.Add(stream);
        await _dbContext.SaveChangesAsync();
        return streamId;
    }

    public async Task EndStream(string streamId)
    {
        var stream = await _dbContext.Streams.Where(s => s.StreamId == streamId).FirstOrDefaultAsync();

        if (stream == null || !stream.IsActive)
        {
            throw new StreamNotLiveException();
        }

        _dbContext.Streams.Remove(stream);
        await _dbContext.SaveChangesAsync();
        //await _hubService.NotifyUsersInStream(body.StreamId);

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
            StreamerName = p.Streamer.FirstName + " " + p.Streamer.LastName,
            StreamerUsername = p.Streamer.Username,
            Thumbnail = p.Thumbnail,
            Users = p.Participants.Select(n => n.User).Select(n => new UserInfoSharedDto()
            {
                FirstName = n.FirstName,
                LastName = n.LastName,
                Username = n.Username,
            }).ToList()
        }).FirstOrDefault();

        return streamInfo;
    }

    public StreamInfoDto? GetStreamInfoBasedOnStreamer(string streamerId)
    {
        var first = _dbContext.Streams.Include(s => s.Streamer);
        var second = first.Where(s => s.Streamer.Username == streamerId);
        var third = second.Select(p => new StreamInfoDto()
        {
            StreamId = p.StreamId,
            isActive = p.IsActive,
            StreamTitle = p.StreamTitle,
            StreamerName = p.Streamer.FirstName + " " + p.Streamer.LastName,
            StreamerUsername = p.Streamer.Username,
            Thumbnail = p.Thumbnail,
            Users = p.Participants.Select(n => n.User).Select(n => new UserInfoSharedDto()
            {
                FirstName = n.FirstName,
                LastName = n.LastName,
                Username = n.Username,
            }).ToList()
        }).FirstOrDefault();

        return third;
    }

    public async Task<List<StreamInfoDto>> GetAllStreams()
    {

        var streams = await _dbContext.Streams.Select(p => new StreamInfoDto()
        {
            StreamId = p.StreamId,
            StreamerName = p.Streamer.FirstName + " " + p.Streamer.LastName,
            StreamerUsername = p.Streamer.Username,
            Thumbnail = p.Thumbnail,
            isActive = p.IsActive,
            StreamTitle = p.StreamTitle,

        }).ToListAsync();

        return streams;

    }
}
