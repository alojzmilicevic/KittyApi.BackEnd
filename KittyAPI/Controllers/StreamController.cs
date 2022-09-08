using KittyApi.Hubs;
using KittyAPI.Dto;
using KittyAPI.Errors;
using KittyAPI.Models;
using KittyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace KittyAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StreamController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly DataContext _dbContext;
    private readonly IUserService _userService;
    private readonly IStreamService _streamService;

    public StreamController(IHubContext<ChatHub> hubContext, DataContext dbContext, 
        [FromServices] IUserService userService, [FromServices] IStreamService streamService)
    {
        _hubContext = hubContext;
        _dbContext = dbContext;
        _userService = userService;
        _streamService = streamService;
    }

    [HttpPost("join-stream")]
    public async Task<IActionResult> JoinStreamAsync([FromBody] UserJoinStreamDto streamInfo)
    {
        var user = _userService.GetUserFromContext(HttpContext);
        var stream = await _dbContext.Streams.Where(s => s.StreamId == streamInfo.StreamId).FirstOrDefaultAsync();

        if (stream == null)
        {
            throw new NoSuchStreamException();
        }

        if (user == null)
        {
            return NotFound("No such user");
        }

        try
        {
            await _dbContext.StreamUsers.AddAsync(new StreamUser()
            {
                StreamId = streamInfo.StreamId,
                UserId = user.UserId,
            });

            await _dbContext.SaveChangesAsync();

            var message = new SignalRMessageDto("incomingCall");
            await _hubContext.Clients.Group(ClientType.Streamer).SendAsync(MessageType.ReceiveMessage, streamInfo.ConnectionId, message);
            return Ok(_streamService.GetStreamInfo(streamInfo.StreamId));

        } catch(DbUpdateException e)
        {
            return BadRequest("User is already in stream");
        }
    }

    [HttpPost("leave-stream")]
    public async Task<IActionResult> LeaveStreamAsync([FromBody] UserJoinStreamDto streamInfo)
    {
        var user = _userService.GetUserFromContext(HttpContext);
        var stream = await _dbContext.Streams.Where(s => s.StreamId == streamInfo.StreamId).FirstOrDefaultAsync();

        if (stream == null)
        {
            throw new NoSuchStreamException();
        }

        if (user == null)
        {
            return NotFound("No such user");
        }

        _dbContext.StreamUsers.Remove(new StreamUser() {
            StreamId = streamInfo.StreamId,
            UserId = user.UserId,
        });

        _dbContext.SaveChanges();

        var message = new SignalRMessageDto("hangup");
        await _hubContext.Clients.Group(ClientType.Streamer).SendAsync(MessageType.ReceiveMessage, streamInfo.ConnectionId, message);

        return Ok(_streamService.GetStreamInfo(streamInfo.StreamId));
    }

    [HttpGet("stream-info/{streamId}")]
    public async Task<IActionResult> StreamInfo(int streamId)
    {
        var streamInfo = _streamService.GetStreamInfo(streamId);

        if (streamInfo == null)
        {
            return NotFound("No such stream " + streamId);
        }

        return Ok(streamInfo);
    }

}
