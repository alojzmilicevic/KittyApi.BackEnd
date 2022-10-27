using System.ComponentModel.DataAnnotations;

namespace KittyAPI.Dto;

public class StreamUserDto
{
    public string? UserId { get; set; }
    [Required]
    public string? StreamId { get; set; }
}
