using KittyAPI.Dto.Resources;
using Microsoft.AspNetCore.Mvc;

namespace KittyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResourcesController : ControllerBase
{
    private readonly DataContext _dbContext;
    public ResourcesController(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("stream-thumbnails")]
    [Produces("application/json", Type = typeof(List<ThumbnailDto>))]
    public async Task<IActionResult> StreamThumbnails()
    {
        List<ThumbnailDto> thumbnails = await _dbContext.Thumbnails
            .Where(p => p.ThumbnailName != "NotFound")
            .Select(p => new ThumbnailDto
            {
                ThumbnailName = p.ThumbnailName,
                ThumbnailPath = p.ThumbnailPath,
                ThumbnailId = p.Id

            })
            .ToListAsync();


        return Ok(thumbnails);
    }
}