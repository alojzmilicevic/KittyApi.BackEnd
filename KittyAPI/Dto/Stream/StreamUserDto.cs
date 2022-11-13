using System.ComponentModel.DataAnnotations;

namespace KittyAPI.Dto.Stream;

public class StreamUserDto
{
    public string ConnectionId { get; set; }
    [Required]
    public string? StreamId { get; set; }
}
