using KittyAPI.Dto;
using KittyAPI.Errors;
using KittyAPI.Models;
using System.Security.Claims;

namespace KittyAPI.Services;

public interface IUserService
{
    public User FindUser(string id);
    public UserDetailDto GetUserFromContext(HttpContext context);
    public void UpdateUser(User user);
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
        if (context.User.Identity is not ClaimsIdentity identity) return null;

        var userClaims = identity.Claims;

        return new UserDetailDto()
        {
            Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Name)?.Value,
            UserId = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
            FirstName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
            LastName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
        };
    }

    public UserDetailDto GetUserById(string id)
    {
        var user = FindUser(id);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        return CreateUserDetailDto(user);
    }

    public UserDetailDto GetUserByUsername(string username)
    {
        var user = _dbContext.Users.Where(x => x.Username == username).SingleOrDefault();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        return CreateUserDetailDto(user);
    }

    public void UpdateUser(User user)
    {
        if (user == null)
        {
            throw new UserNotFoundException();
        }

        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
    }

    private UserDetailDto CreateUserDetailDto(User user)
    {
        return new UserDetailDto()
        {
            Username = user.Username,
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
    }
}
