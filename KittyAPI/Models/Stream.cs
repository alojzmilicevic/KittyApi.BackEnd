using System.ComponentModel.DataAnnotations;

namespace KittyAPI.Models;

public class Stream
{
    [Key]
    public string StreamId { get; set; }
    public string StreamTitle { get; set; }
    public bool IsActive { get; set; }
    public IList<StreamUser> Participants { get; set; }
    public User Streamer { get; set; }
    public Thumbnail Thumbnail { get; set; }
}
