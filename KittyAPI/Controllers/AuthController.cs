using KittyAPI.Dto;
using KittyAPI.Errors;
using KittyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KittyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private IAuthService _authService;
    private IUserService _userService;
    public AuthController([FromServices] IAuthService authService, [FromServices] IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto userLogin)
    {
        var user = await _authService.AuthenticateAsync(userLogin);

        if (user != null)
        {
            var token = _authService.GenerateToken(user);
            var userDetail = _userService.GetUserById(user.UserId);

            var response = new
            {
                token,
                user = userDetail
            };
            return Ok(response);
        }

        throw new UserNotFoundException();
    }
}
