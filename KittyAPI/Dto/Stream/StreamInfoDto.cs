using KittyAPI.Models;

namespace KittyAPI.Dto.Stream;

public class StreamInfoDto
{
    public string StreamId { get; set; }
    public string StreamTitle { get; set; }
    public string StreamerName { get; set; }
    public string StreamerUsername { get; set; }
    public bool isActive { get; set; }
    public List<UserInfoSharedDto> Users { get; set; }
    public Thumbnail Thumbnail { get; set; }
}
