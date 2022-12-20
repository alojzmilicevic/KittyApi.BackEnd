using KittyAPI.Dto;
using KittyAPI.Models;
using KittyAPI.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KittyAPI.Services;

public interface IAuthService
{
    string GenerateToken(User user);
    Task<User> AuthenticateAsync(UserLoginDto userLogin);
}

public class AuthService : IAuthService
{
    private readonly DataContext _dbContext;
    private readonly JwtSettings _jwtSettings;

    public AuthService(DataContext dbContext, IOptions<JwtSettings> options)
    {
        _dbContext = dbContext;
        _jwtSettings = options.Value;
    }

    public async Task<User> AuthenticateAsync(UserLoginDto userLogin)
    {
        var user = await _dbContext.Users.Where(x => x.Email == userLogin.UserName).SingleOrDefaultAsync();

        if (user == null) return null;

        var validPassword = PasswordService.VerifyHashSHA512(userLogin.Password, Convert.FromBase64String(user.PasswordHash), Convert.FromBase64String(user.PasswordSalt));

        if (!validPassword) return null;

        return user;
    }

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
        };

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
