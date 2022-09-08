using System.ComponentModel.DataAnnotations.Schema;

namespace KittyAPI.Models;

public class Stream
{
    public int StreamId { get; set; }
    public string StreamTitle { get; set; }
    public bool IsActive { get; set; }
    public IList<StreamUser> Participants { get; set; }

}
