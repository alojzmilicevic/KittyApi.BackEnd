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
    public AuthController([FromServices] IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto userLogin)
    {
        var user = await _authService.AuthenticateAsync(userLogin);

        if (user != null)
        {
            var token = _authService.GenerateToken(user);

            return Ok(token);
        }

        throw new UserNotFoundException();
    }
}
