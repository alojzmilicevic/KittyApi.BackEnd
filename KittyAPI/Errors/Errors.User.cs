using System.Net;

namespace KittyAPI.Errors;

public class UserNotFoundException : Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.NotFound;

    public string ErrorCode => "User.NotFound";
    public string ErrorMessage => "That user doesn't exist"; 
}

public class UserExistsException : Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public string ErrorMessage => "User already registered";

    public string ErrorCode => "User.AlreadyExists";
}

public class UserAlreadyInStreamException: Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public string ErrorMessage => "User is already in stream";

    public string ErrorCode => "User.AlreadyInStream";
}

