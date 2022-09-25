using System.Net;

namespace KittyAPI.Errors;

public class StreamNotFoundException : Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    public string ErrorCode => "Stream.NotFound";

    public string ErrorMessage => "Stream not found";

}

public class StreamAlreadyLiveException : Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

    public string ErrorCode => "Stream.AlreadyLive";
    public string ErrorMessage => "Stream is already live";

}

public class StreamNotLiveException : Exception, IServiceException
{
    public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    public string ErrorCode => "Stream.NotLive";
    public string ErrorMessage => "Stream is not live";
}

