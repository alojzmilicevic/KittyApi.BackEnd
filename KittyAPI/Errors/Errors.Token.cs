using System.Net;

namespace KittyAPI.Errors;

public class TokenValidationException : Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
    public string ErrorCode => "Token.NotValid";
    public string ErrorMessage => "Token validation failed";
}
