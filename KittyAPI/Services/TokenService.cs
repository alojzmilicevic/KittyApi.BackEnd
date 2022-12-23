using KittyAPI.Dto.Auth;
using KittyAPI.Errors;
using KittyAPI.Models;
using KittyAPI.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KittyAPI.Services;

public interface ITokenService
{
    public AuthenticationResult GenerateToken(User user);
    public string ExtractUserIdFromOldToken(string token);
}

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
    }

    public string ExtractUserIdFromOldToken(string token)
    {
        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token, out bool isTokenValid);

        if (!isTokenValid)
        {
            throw new TokenValidationException();
        }

        var userId = principal.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;

        return userId;
    }

    public AuthenticationResult GenerateToken(User user)
    {

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            GenerateClaimsFromUser(user),
            signingCredentials: credentials,
            expires: DateTime.Now.AddSeconds(_jwtSettings.Expiry)
        );

        return new AuthenticationResult()
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = Guid.NewGuid().ToString(),
            Expiry = token.ValidTo
        };
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token, out bool isTokenValid)
    {
        isTokenValid = false;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key)
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture))
                throw new TokenValidationException();

            isTokenValid = true;
            return principal;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static Claim[] GenerateClaimsFromUser(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
        };

        return claims;
    }
}
