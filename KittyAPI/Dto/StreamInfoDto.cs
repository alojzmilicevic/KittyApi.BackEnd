namespace KittyAPI.Dto;

public class StreamInfoDto
{
    public string StreamId { get; set; }
    public string StreamerId { get; set; }
    public string StreamTitle { get; set; }
    public bool isActive { get; set; }
    public List<UserInfoSharedDto> Users { get; set; }
}
