namespace KittyAPI.Models;

public class StreamUser
{
    public string StreamId { get; set; }
    public Stream Stream { get; set; }

    
    public string UserId { get; set; }
    public User User { get; set; }
}
