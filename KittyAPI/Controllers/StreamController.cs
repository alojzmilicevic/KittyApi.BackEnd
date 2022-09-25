using KittyApi.Hubs;
using KittyAPI.Dto;
using KittyAPI.Errors;
using KittyAPI.Hubs;
using KittyAPI.Models;
using KittyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KittyAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StreamController : ControllerBase
{
    private readonly DataContext _dbContext;
    private readonly IHubService _hubService;
    private readonly IUserService _userService;
    private readonly IStreamService _streamService;

    public StreamController([FromServices] IHubService hubService, DataContext dbContext, 
        [FromServices] IUserService userService, [FromServices] IStreamService streamService)
    {
        _hubService = hubService;
        _dbContext = dbContext;
        _userService = userService;
        _streamService = streamService;
    }

    [HttpPost("join-stream")]
    public async Task<IActionResult> JoinStreamAsync([FromBody] UserJoinStreamDto streamInfo)
    {
        var user = _userService.GetUserFromContext(HttpContext);
        await checkIfStreamExistsAndIsRunning(streamInfo.StreamId);

        var stream = await _streamService.AddUserToStream(user, streamInfo.StreamId);

        return Ok(stream);
    }

    [HttpPost("leave-stream")]
    public async Task<IActionResult> LeaveStreamAsync([FromBody] UserJoinStreamDto streamInfo)
    {
        var user = _userService.GetUserFromContext(HttpContext);

        await checkIfStreamExistsAndIsRunning(streamInfo.StreamId);

        await _streamService.KickUserFromStream(user, streamInfo.StreamId);

        return Ok(_streamService.GetStreamInfo(streamInfo.StreamId));
    }

    [HttpGet("stream-info/{streamId}")]
    public async Task<IActionResult> StreamInfo(int streamId)
    {
        var streamInfo = _streamService.GetStreamInfo(streamId);

        if (streamInfo == null)
        {
            throw new StreamNotFoundException();
        }

        return Ok(streamInfo);
    }

    [HttpPost("kick-user")]
    public async Task<IActionResult> KickUser([FromBody] UserJoinStreamDto body)
    {
        var user = _userService.FindUser(body.UserId);
        /*
         if(isInStream(user)) {
            await _streamService.KickUserFromStream(user, body.StreamId)
            notify user that user has been kicked
            notifyStreamer that user has been kicked
         } else {
            return Error to streamer
        }
         */
        return Ok("");
    }

    [HttpPost("start-stream")]
    public async Task<IActionResult> StartStream([FromBody] UserJoinStreamDto body)
    {
        // TODO: Notify all users stream has started
        await _streamService.StartStream(body);

        return Ok("Started stream");
    }

    
    [HttpPost("end-stream")]
    public async Task<IActionResult> EndStream([FromBody] UserJoinStreamDto body)
    {
        // TODO: Notify all users stream has ended
        // Remove users from channel?
        await _streamService.EndStream(body);

        return Ok("Stream ended");
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task checkIfStreamExistsAndIsRunning(int streamId) {
        var stream = await _dbContext.Streams.Where(s => s.StreamId == streamId).FirstOrDefaultAsync();

        if (stream == null)
        {
            throw new StreamNotFoundException();
        }

        if(!stream.IsActive)
        {
            throw new StreamNotLiveException();
        }
    }
}
