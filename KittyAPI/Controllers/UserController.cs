using System.Security.Claims;
using KittyAPI.Dto;
using KittyAPI.Models;
using KittyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KittyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private IUserService _userService;
    private readonly DataContext _dbContext;

    public UserController([FromServices] IUserService userService, DataContext dbContext)
    {
        _userService = userService;
        _dbContext = dbContext;

    }

    [Authorize]
    [HttpGet]
    public IActionResult GetUser()
    {
        var currentUser = _userService.GetUserFromContext(HttpContext);

        if (currentUser == null) return NotFound("User doesnt exist");

        return Ok(currentUser);
    }


    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegister)
    {
        // TODO Check if password format is ok 
        // TODO check if email is a real email

        var userExists = await _dbContext.Users.AnyAsync(x => x.Email == userRegister.Email || x.Username == userRegister.Username);

        if (userExists)
        {
            return BadRequest("User already exists");
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
            return BadRequest("User not found.");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return Ok(await _dbContext.Users.ToListAsync());
    }



}
