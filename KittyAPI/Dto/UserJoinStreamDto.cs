using System.ComponentModel.DataAnnotations;

namespace KittyAPI.Dto;

public class UserJoinStreamDto
{
    public string? UserId { get; set; }
    [Required]
    public int StreamId { get; set; }
}
