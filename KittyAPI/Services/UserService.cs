using KittyAPI.Dto;
using KittyAPI.Errors;
using KittyAPI.Models;
using System.Security.Claims;

namespace KittyAPI.Services;

public interface IUserService
{
    User FindUser(string id);
    UserDetailDto GetUserFromContext(HttpContext context);
    UserDetailDto GetUserById(string userId);
}
public class UserService : IUserService
{
    private readonly DataContext _dbContext;
    public UserService(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public User FindUser(string id)
    {
        return _dbContext.Users.Find(id);
    }

    public UserDetailDto GetUserFromContext(HttpContext context)
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

    public UserDetailDto GetUserById(string id)
    {
        var user = FindUser(id);

        if (user == null)
        {
            throw new UserNotFoundException();
        }
        
        return new UserDetailDto()
        {
            Username = user.Username,
            UserId = user.UserId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
        };
    }
}
