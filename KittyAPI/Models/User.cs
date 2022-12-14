using System.ComponentModel.DataAnnotations;

namespace KittyAPI.Models;

public class User
{
    [Key]
    public string UserId { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public string RefreshToken { get; set; }
    public IList<StreamUser> Streams { get; set; }
}
