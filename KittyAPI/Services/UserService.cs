using KittyAPI.Dto;
using KittyAPI.Models;
using System.Security.Claims;

namespace KittyAPI.Services;

public interface IUserService
{
    User? FindUser(string id);
    UserDetailDto? GetUserFromContext(HttpContext context);
}
public class UserService : IUserService
{
    private readonly DataContext _context;
    public UserService(DataContext context)
    {
        _context = context;
    }

    public User? FindUser(string id)
    {
        return _context.Users.Find(id);
    }

    public UserDetailDto? GetUserFromContext(HttpContext context)
    {
        var identity = context.User.Identity as ClaimsIdentity;

        if (identity == null) return null;

        var userClaims = identity.Claims;

        return new UserDetailDto()
        {
            Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
            UserId = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
            Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
            FirstName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
            LastName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
            Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
        };
    }

}
