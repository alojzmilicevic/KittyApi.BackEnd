using KittyAPI.Dto;
using Microsoft.AspNetCore.Mvc;

namespace KittyAPI.Services;

public interface IStreamService
{
    public StreamInfoDto? GetStreamInfo(int streamId);
    StreamInfoDto? KickUserFromStream(HttpContext context, int streamId, string connectionId);
}
public class StreamService : IStreamService
{
    private readonly DataContext _dbContext;
    private readonly IUserService _userService;

    public StreamService(DataContext dbContext, [FromServices] IUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public StreamInfoDto? KickUserFromStream(HttpContext context, int streamId, string connectionId)
    {
        var user = _userService.GetUserFromContext(context);
        return null;
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
}
