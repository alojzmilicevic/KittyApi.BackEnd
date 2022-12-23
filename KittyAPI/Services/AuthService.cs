using KittyAPI.Dto;
using KittyAPI.Dto.Auth;
using KittyAPI.Models;

namespace KittyAPI.Services;

public interface IAuthService
{
    Task<User> Authenticate(UserLoginDto userLogin);
    User GetUserFromOldAuthenticationResult(AuthenticationResult oldAuth);
}

public class AuthService : IAuthService
{
    private readonly DataContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public AuthService(DataContext dbContext, ITokenService tokenService, IUserService userService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _userService = userService;
    }

    public async Task<User> Authenticate(UserLoginDto userLogin)
    {
        var user = await _dbContext.Users.Where(x => x.Username== userLogin.Username).SingleOrDefaultAsync();

        if (user == null) return null;

        var validPassword = PasswordService.VerifyHashSHA512(
            userLogin.Password,
            Convert.FromBase64String(user.PasswordHash),
            Convert.FromBase64String(user.PasswordSalt)
        );

        if (!validPassword) return null;

        return user;
    }

    public User GetUserFromOldAuthenticationResult(AuthenticationResult oldAuth)
    {
        var userId = _tokenService.ExtractUserIdFromOldToken(oldAuth.AccessToken);

        if (userId == null) return null;

        User user = _userService.FindUser(userId);

        if (user == null) return null;

        // Check if the refresh token matches the one in the database
        // Future improvement: If the refresh token doesn't match the users token
        // then we probably have someone trying to hack the system using a stolen token.
        if (user.RefreshToken != oldAuth.RefreshToken) return null;

        return user;
    }
}
