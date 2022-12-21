using KittyAPI.Dto;
using KittyAPI.Dto.Auth;
using KittyAPI.Errors;
using KittyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KittyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController([FromServices] IAuthService authService, [FromServices] IUserService userService, ITokenService tokenService)
    {
        _authService = authService;
        _userService = userService;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
    {
        var user = await _authService.Authenticate(userLogin);

        if (user == null) throw new UserNotFoundException();

        AuthenticationResult authResult = _tokenService.GenerateToken(user);

        user.RefreshToken = authResult.RefreshToken;
        _userService.UpdateUser(user);

        return Ok(authResult);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public IActionResult RefreshToken([FromBody] AuthenticationResult oldAuth)
    {
        var user = _authService.GetUserFromOldAuthenticationResult(oldAuth);

        if (user == null) throw new TokenValidationException();

        AuthenticationResult auth = _tokenService.GenerateToken(user);

        user.RefreshToken = auth.RefreshToken;
        _userService.UpdateUser(user);

        return Ok(auth);
    }
}
