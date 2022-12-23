using KittyAPI.Dto;
using KittyAPI.Dto.Auth;
using KittyAPI.Errors;
using KittyAPI.Models;
using KittyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KittyAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly DataContext _dbContext;
    private readonly ITokenService _tokenService;

    public UserController([FromServices] IUserService userService, DataContext dbContext, ITokenService tokenService)
    {
        _userService = userService;
        _dbContext = dbContext;
        _tokenService = tokenService;
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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegister)
    {
        var userExists = await _dbContext.Users.AnyAsync(x => x.Username == userRegister.Username);

        if (userExists)
        {
            throw new UserExistsException();
        }

        PasswordService.ComputeHashSHA512(userRegister.Password, out byte[] passwordHash, out byte[] passwordSalt);
        var user = new User()
        {
            Username = userRegister.Username,
            UserId = Guid.NewGuid().ToString(),
            FirstName = userRegister.FirstName,
            LastName = userRegister.LastName,
            PasswordHash = Convert.ToBase64String(passwordHash),
            PasswordSalt = Convert.ToBase64String(passwordSalt),
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

        AuthenticationResult authResult = _tokenService.GenerateToken(user);

        return Ok(new RelogDto()
        {
            AuthResult = authResult,
            User = new UserDetailDto()
            {
                Username = user.Username,
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
            }
        });
    }
}
