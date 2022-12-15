using System.Security.Claims;
using KittyAPI.Dto;
using KittyAPI.Errors;
using KittyAPI.Models;
using KittyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KittyAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private IUserService _userService;
    private readonly IAuthService _authService;
    private readonly DataContext _dbContext;

    public UserController([FromServices] IUserService userService, [FromServices] IAuthService authService, DataContext dbContext)
    {
        _userService = userService;
        _authService = authService;
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult GetUser()
    {
        var currentUser = _userService.GetUserFromContext(HttpContext);

        if (currentUser == null)
        {
            throw new UserNotFoundException();
        }

        return Ok(currentUser);
    }


    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegister)
    {
        var userExists = await _dbContext.Users.AnyAsync(x => x.Email == userRegister.Email || x.Username == userRegister.Username);

        if (userExists)
        {
            throw new UserExistsException();
        }

        PasswordService.ComputeHashSHA512(userRegister.Password, out byte[] passwordHash, out byte[] passwordSalt);
        var user = new User()
        {
            Email = userRegister.Email,
            Username = userRegister.Username,
            UserId = Guid.NewGuid().ToString(),
            FirstName = userRegister.FirstName,
            LastName = userRegister.LastName,
            PasswordHash = Convert.ToBase64String(passwordHash),
            PasswordSalt = Convert.ToBase64String(passwordSalt),
            Role = "User",
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return Ok(user);
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult<List<User>>> Delete(string id)
    {
        var user = _userService.FindUser(id);

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return Ok(await _dbContext.Users.ToListAsync());
    }

    [HttpGet("check-username")]
    public async Task<ActionResult<UserInfoSharedDto>> CheckUsername(string username)
    {
        var userExists = await _dbContext.Users.AnyAsync(x => x.Username == username);

        return Ok(userExists);
    }

    [HttpPost("change-username")]
    public async Task<ActionResult> ChangeUsername(string username)
    {
        var currentUser = _userService.GetUserFromContext(HttpContext);

        if (currentUser == null)
        {
            throw new UserNotFoundException();
        }

        var user = await _dbContext.Users.Where(u => u.Username == currentUser.Username).FirstOrDefaultAsync();

        if (user == null)
        {
            throw new UserNotFoundException();
        }

        user.Username = username;
        _dbContext.Update(user);
        _dbContext.SaveChanges();

        var token = _authService.GenerateToken(user);

        return Ok(new RelogDto()
        {
            Token = token,
            User = new UserDetailDto()
            {
                Username = user.Username,
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            }
        });
    }
}
