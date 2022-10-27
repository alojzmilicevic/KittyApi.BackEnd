using KittyAPI.Dto;
using KittyAPI.Dto.Stream;
using KittyAPI.Errors;
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
    private readonly IUserService _userService;
    private readonly IStreamService _streamService;

    public StreamController(DataContext dbContext,
        [FromServices] IUserService userService, [FromServices] IStreamService streamService)
    {
        _dbContext = dbContext;
        _userService = userService;
        _streamService = streamService;
    }

    [HttpPost("join-stream/{streamId}")]
    public async Task<IActionResult> JoinStreamAsync(string streamId)
    {
        var user = _userService.GetUserFromContext(HttpContext);
        await checkIfStreamExistsAndIsRunning(streamId);

        var stream = await _streamService.AddUserToStream(user, streamId);

        return Ok(stream);
    }

    [HttpPost("leave-stream/{streamId}")]
    public async Task<IActionResult> LeaveStreamAsync(string streamId)
    {
        var user = _userService.GetUserFromContext(HttpContext);

        await checkIfStreamExistsAndIsRunning(streamId);

        await _streamService.KickUserFromStream(user, streamId);

        return Ok(_streamService.GetStreamInfo(streamId));
    }

    [HttpGet("stream-info/{streamId}")]
    public async Task<IActionResult> StreamInfo(string streamId)
    {
        var streamInfo = _streamService.GetStreamInfo(streamId);

        if (streamInfo == null)
        {
            throw new StreamNotFoundException();
        }

        return Ok(streamInfo);
    }

    [HttpGet("stream-info")]
    public async Task<IActionResult> AllStreams()
    {
        var streams = await _streamService.GetAllStreams();

        return Ok(streams);
    }

    [HttpPost("kick-user")]
    public async Task<IActionResult> KickUser([FromBody] StreamUserDto body)
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
    public async Task<IActionResult> StartStream([FromBody] StartStreamDto body)
    {
        var user = _userService.GetUserFromContext(HttpContext);

        // TODO: Notify all users stream has started
        await _streamService.StartStream(body, user);

        return Ok("Started stream");
    }


    [HttpPost("end-stream/{streamId}")]
    public async Task<IActionResult> EndStream(string streamId)
    {
        // TODO: Notify all users stream has ended
        // Remove users from channel?
        await _streamService.EndStream(streamId);

        return Ok("Stream ended");
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task checkIfStreamExistsAndIsRunning(string streamId)
    {
        var stream = await _dbContext.Streams.Where(s => s.StreamId == streamId).FirstOrDefaultAsync();

        if (stream == null)
        {
            throw new StreamNotFoundException();
        }

        if (!stream.IsActive)
        {
            throw new StreamNotLiveException();
        }
    }
}
