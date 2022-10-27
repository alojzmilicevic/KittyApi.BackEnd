using System.ComponentModel.DataAnnotations;

namespace KittyAPI.Models;

public class Stream
{
    [Key]
    public string StreamId { get; set; }
    public string StreamTitle { get; set; }
    public bool IsActive { get; set; }
    public IList<StreamUser> Participants { get; set; }
    public string StreamerId { get; set; }

}
